using FileCastle.constants;
using FileCastle.helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static FileCastle.constants.FileCastleConstants;

namespace FileCastle.service
{
    class FileCastleService
    {
        // TODO: Major code cleanup
        // TODO: Testing edge-cases of filename logic
        // TODO: Run benchmarks with different buffer sizes
        // TODO: Add exception handling
        // TODO: Add more button functionality
        // TODO: Replace messages with constants
        // TODO: Use compression for portability
        // TODO: Add comments for readability
        // TODO: Use LINQ wherever possible

        public delegate void FileProcessedEventHandler(object sender, string fileName);
        public event FileProcessedEventHandler FileProcessed;

        #region "File Extension Verification"
        public Tuple<Enums.Actions, long> VerifyAllFiles(List<string> _filesToConsider)
        {
            int filesCount = _filesToConsider.Count, encryptedFilesCount = 0;
            long totalFileBytes = 0;
            //Tuple<Enums.Actions, long> response;
            foreach (string fileName in _filesToConsider)
            {
                VerifyCurrentFile(fileName, ref encryptedFilesCount, ref totalFileBytes);
            }

            if ((encryptedFilesCount > 0) && (encryptedFilesCount >= filesCount))
            {
                //return Enums.Actions.Decrypt;
                return new Tuple<Enums.Actions, long>(Enums.Actions.Decrypt, totalFileBytes);
            }
            else if (encryptedFilesCount == 0)
            {
                return new Tuple<Enums.Actions, long>(Enums.Actions.Encrypt, totalFileBytes);
            }
            else
            {
                throw new Exception("Invalid selection of files.");
            }
        }

        private void VerifyCurrentFile(string _fileName, ref int _encryptedFilesCount, ref long _totalFileBytes)
        {
            if (Directory.Exists(_fileName))
            {
                DirectoryInfo dir = new DirectoryInfo(_fileName);
                var files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    VerifyCurrentFile(file.FullName, ref _encryptedFilesCount, ref _totalFileBytes);
                }
            }
            else
            {
                if (new FileInfo(_fileName).Extension == ENCRYPTED_EXTENSION)
                {
                    _encryptedFilesCount++;
                }
                _totalFileBytes += new FileInfo(_fileName).Length;
            }
        }
        #endregion

        #region "Encryption/Decryption"
        public void ProcessFiles(IProgress<int> _progress, List<string> _filesToProcess, string _key, Tuple<Enums.Actions, long> _actionInfo)
        {
            long totalFileBytes = _actionInfo.Item2, totalProcessedBytes = 0;
            foreach (string fileName in _filesToProcess)
            {
                if (_actionInfo.Item1 == Enums.Actions.Encrypt)
                    EncryptFile(fileName, _key, totalFileBytes, ref totalProcessedBytes, ref _progress);
                else
                    DecryptFile(fileName, _key, totalFileBytes, ref totalProcessedBytes, ref _progress);

                OnFileProcessed(fileName);
            }

            // After all the files have been processed, reset the progressbar value to 0.
            // Update status text etc here
            _progress.Report(0);
        }

        private void EncryptFile(string _fileName, string _key, long _totalFileBytes, ref long totalProcessedBytes, ref IProgress<int> _progress)
        {
            if (Directory.Exists(_fileName))
            {
                DirectoryInfo dir = new DirectoryInfo(_fileName);
                foreach (FileInfo file in dir.GetFiles())
                {
                    EncryptFile(file.FullName, _key, _totalFileBytes, ref totalProcessedBytes, ref _progress);
                }
                // Encrypt each file within sub-directories recursively
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    EncryptFile(subDir.FullName, _key, _totalFileBytes, ref totalProcessedBytes, ref _progress);
                }

                // Generate a random directory that doesn't already exist
                string newDirName;
                do
                {
                    newDirName = FileCastleUtil.GenerateRandomFileName();
                } while (Directory.Exists(newDirName));

                // Write the original (encrypted) directory name into the fcMeta file
                File.WriteAllText(Path.Combine(dir.FullName, FC_META_INFO),
                    Convert.ToBase64String(AES.Encrypt(ASCIIEncoding.ASCII.GetBytes(_fileName), _key)));
                dir.MoveTo(Path.Combine(dir.Parent.FullName, newDirName));
            }
            else
            {
                FileInfo curFile = new FileInfo(_fileName);

                byte[] rawBuffer = new byte[BUFFER_SIZE_STANDARD];
                byte[] fileNameBytes = ASCIIEncoding.ASCII.GetBytes(curFile.Name);
                byte[] encryptedFileNameBytes = AES.Encrypt(fileNameBytes, _key);

                /*
                 * encryptedFileNameLengthBytes is used for storing the length of the AES encrypted file name
                 * Maximum file path length for windows is 260 characters. i.e: the length can be safely stored using 3 (FILE_NAME_LENGTH_CHUNK_SIZE) bytes
                 */
                byte[] encryptedFileNameLengthBytes = new byte[FILE_NAME_LENGTH_CHUNK_SIZE];

                // Store the length in a buffer and copy it to encryptedFileNameLengthBytes so that the right-padding will remain
                byte[] fileNameLengthBuffer = ASCIIEncoding.ASCII.GetBytes(encryptedFileNameBytes.Length.ToString());
                System.Buffer.BlockCopy(fileNameLengthBuffer, 0, encryptedFileNameLengthBytes, 0, fileNameLengthBuffer.Length);

                // Append encryptedFileNameLengthBytes to the beginning of the actual encryptedFileNameBytes
                encryptedFileNameBytes = encryptedFileNameLengthBytes.Concat(encryptedFileNameBytes).ToArray();
                System.Buffer.BlockCopy(encryptedFileNameBytes, 0, rawBuffer, 0, encryptedFileNameBytes.Length);

                // Generate a random Windows-safe file name that doesn't exist at that path
                string encFileName;
                do
                {
                    encFileName = FileCastleUtil.GenerateRandomFileName();
                } while (File.Exists(encFileName));
                string fullPath = curFile.FullName.Replace(curFile.Name, encFileName);

                FileStream fsWrite = new FileStream(fullPath, FileMode.Append, FileAccess.Write);
                FileStream fsRead = new FileStream(curFile.FullName, FileMode.Open, FileAccess.Read);
                BufferedStream bsRead = new BufferedStream(fsRead, BUFFER_SIZE_STANDARD);

                // Write the fileName info to disk
                fsWrite.Write(rawBuffer, 0, rawBuffer.Length);
                Array.Clear(rawBuffer, 0, rawBuffer.Length);

                long totalBytesRead = 0;
                while (totalBytesRead < curFile.Length)
                {
                    int bytesRead = bsRead.Read(rawBuffer, 0, rawBuffer.Length);
                    totalBytesRead += bytesRead;

                    // bytesRead will be less than rawBuffer.Length for the last chunk, we need to handle that separately as follows
                    if (bytesRead != rawBuffer.Length)
                    {
                        // Need to update rawBuffer's size to bytesRead so we don't write empty bytes onto disk
                        byte[] swap = rawBuffer;
                        rawBuffer = new byte[bytesRead];
                        System.Buffer.BlockCopy(swap, 0, rawBuffer, 0, bytesRead);
                    }
                    if (rawBuffer.Length > 0)
                    {
                        byte[] encryptedBytes = AES.Encrypt(rawBuffer, _key);
                        fsWrite.Write(encryptedBytes, 0, encryptedBytes.Length);
                    }

                    // Updating the progressbar
                    totalProcessedBytes += bytesRead;
                    int progressPercent = (int)((totalProcessedBytes * 100) / _totalFileBytes);
                    _progress.Report(progressPercent);

                    // Clear rawBuffer
                    Array.Clear(rawBuffer, 0, rawBuffer.Length);
                }

                //MessageBox.Show("Encryption done");

                fsWrite.Close(); fsRead.Close(); bsRead.Close();

                // Delete original file only after writing encrypted file to disk
                File.Delete(curFile.FullName);
            }
        }

        private void DecryptFile(string fileName, string key, long _totalFileBytes, ref long totalProcessedBytes, ref IProgress<int> _progress)
        {
            if (Directory.Exists(fileName))
            {
                DirectoryInfo dir = new DirectoryInfo(fileName);
                foreach (FileInfo file in dir.GetFiles())
                {
                    DecryptFile(file.FullName, key, _totalFileBytes, ref totalProcessedBytes, ref _progress);
                }

                /*
                 * We take a top-down approach while decrypting the directory names.
                 * Bottom-up approach won't work since the parent directory name would still be encrypted, and hence the path of the subdirectory would be invalid.
                 */
                if (File.Exists(Path.Combine(dir.FullName, FC_META_INFO)))
                {
                    string encryptedDirName = File.ReadAllText(Path.Combine(dir.FullName, FC_META_INFO));
                    string rawDirName = ASCIIEncoding.ASCII.GetString(AES.Decrypt(Convert.FromBase64String(encryptedDirName), key));
                    dir.MoveTo(Path.Combine(dir.Parent.FullName, rawDirName));
                    File.Delete(Path.Combine(dir.FullName, FC_META_INFO));
                }

                // Decrypt name of parent directory before going into subdirectories
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    DecryptFile(subDir.FullName, key, _totalFileBytes, ref totalProcessedBytes, ref _progress);
                }
            }
            else
            {
                FileInfo curFile = new FileInfo(fileName);
                if (curFile.Extension == ENCRYPTED_EXTENSION)
                {
                    byte[] encBuffer = new byte[BUFFER_SIZE_STANDARD];
                    FileStream fsRead = new FileStream(fileName, FileMode.Open);
                    BufferedStream bsRead = new BufferedStream(fsRead);
                    bsRead.Read(encBuffer, 0, BUFFER_SIZE_STANDARD);
                    byte[] fileNameLengthBytes = new byte[FILE_NAME_LENGTH_CHUNK_SIZE];

                    // Copy the first 3 (FILE_NAME_LENGTH_CHUNK_SIZE) bytes of encBuffer to fileNameLengthBytes
                    System.Buffer.BlockCopy(encBuffer, 0, fileNameLengthBytes, 0, FILE_NAME_LENGTH_CHUNK_SIZE);
                    int fileNameLength = Convert.ToInt32(Encoding.ASCII.GetString(fileNameLengthBytes));

                    // Create byte array of size fileNameLength to hold the encrypted fileName bytes
                    byte[] encryptedFileNameBytes = new byte[fileNameLength];

                    // Start copying from position {3} to skip the filename length
                    System.Buffer.BlockCopy(encBuffer, FILE_NAME_LENGTH_CHUNK_SIZE, encryptedFileNameBytes, 0, fileNameLength);
                    byte[] decryptedFileNameBytes = AES.Decrypt(encryptedFileNameBytes, key);
                    string rawFileName = FileCastleUtil.CleanFileName(ASCIIEncoding.ASCII.GetString(decryptedFileNameBytes));
                    FileStream fsWrite = new FileStream(curFile.FullName.Replace(curFile.Name, rawFileName), FileMode.Append, FileAccess.Write);

                    // Initialize totalBytesWritten as BUFFER_SIZE_STANDARD (8192) to account for the first chunk (fileName chunk)
                    long totalBytesWritten = BUFFER_SIZE_STANDARD;

                    // Encryption result gets padded with 16 more bytes (8192 + 16)
                    encBuffer = new byte[BUFFER_SIZE_PADDED];
                    while (totalBytesWritten < curFile.Length)
                    {
                        int bytesRead = bsRead.Read(encBuffer, 0, encBuffer.Length);
                        totalBytesWritten += bytesRead;

                        // For the last chunk
                        if (bytesRead != encBuffer.Length)
                        {
                            byte[] swap = encBuffer;
                            encBuffer = new byte[bytesRead];
                            System.Buffer.BlockCopy(swap, 0, encBuffer, 0, bytesRead);
                        }
                        if (encBuffer.Length > 0)
                        {
                            byte[] decryptedBytes = AES.Decrypt(encBuffer, key);
                            fsWrite.Write(decryptedBytes, 0, decryptedBytes.Length);
                        }

                        // Updating the progressbar
                        totalProcessedBytes += bytesRead;
                        int progressPercent = (int)((totalProcessedBytes * 100) / _totalFileBytes);
                        _progress.Report(progressPercent);

                        Array.Clear(encBuffer, 0, encBuffer.Length);
                    }
                    //MessageBox.Show("Finished writing.");
                    fsWrite.Close(); bsRead.Close(); fsRead.Close();

                    // Delete encrypted file after successful decryption
                    File.Delete(fileName);
                }
            }
        }
        #endregion

        protected virtual void OnFileProcessed(string _fileName)
        {
            FileProcessed?.Invoke(this, _fileName);
        }
    }
}
