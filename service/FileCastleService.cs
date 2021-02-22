using FileCastle.constants;
using FileCastle.helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileCastle.service
{
    class FileCastleService
    {
        // TODO: Major code cleanup
        // TODO: Testing edge-cases of filename logic
        // TODO: Add exception handling
        // TODO: Add more button functionality
        // TODO: Improved progress bar
        // TODO: Replace messages with constants
        // TODO: Cleanup filenames before writing to disk
        // TODO: Use compression for portability
        // TODO: Add comments for readability
        // TODO: Use LINQ wherever possible

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
                if (new FileInfo(_fileName).Extension == FileCastleConstants.ENCRYPTED_EXTENSION)
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
            //int filesCount = 0, percent;
            long totalFileBytes = _actionInfo.Item2, totalProcessedBytes = 0;
            foreach (string fileName in _filesToProcess)
            {
                if (_actionInfo.Item1 == Enums.Actions.Encrypt)
                    EncryptFile(fileName, _key, totalFileBytes, ref totalProcessedBytes, ref _progress);
                else
                    DecryptFile(fileName, _key, totalFileBytes, ref totalProcessedBytes, ref _progress);
/*                filesCount++;
                percent = (filesCount * 100) / _filesToProcess.Count;
                _progress.Report(percent);*/
            }
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
                File.WriteAllText(Path.Combine(dir.FullName, FileCastleConstants.FC_META_INFO), 
                    Convert.ToBase64String(AES.Encrypt(ASCIIEncoding.ASCII.GetBytes(_fileName), _key)));
                dir.MoveTo(Path.Combine(dir.Parent.FullName, newDirName));
            }
            else
            {
                FileInfo curFile = new FileInfo(_fileName);

                byte[] rawBuffer = new byte[FileCastleConstants.BUFFER_SIZE_STANDARD];
                
                byte[] fileNameBytes = ASCIIEncoding.ASCII.GetBytes(curFile.Name);
                //Encrypt fileNameBytes[]
                byte[] encryptedFileNameBytes = AES.Encrypt(fileNameBytes, _key);
                byte[] encryptedFileNameLengthBytes = new byte[3];
                //encryptedFileNameLengthBytes = ASCIIEncoding.ASCII.GetBytes(encryptedFileNameBytes.Length.ToString());
                byte[] fileNameLengthBuffer = ASCIIEncoding.ASCII.GetBytes(encryptedFileNameBytes.Length.ToString());
                System.Buffer.BlockCopy(fileNameLengthBuffer, 0, encryptedFileNameLengthBytes, 0, fileNameLengthBuffer.Length);
                encryptedFileNameBytes = encryptedFileNameLengthBytes.Concat(encryptedFileNameBytes).ToArray();
                System.Buffer.BlockCopy(encryptedFileNameBytes, 0, rawBuffer, 0, encryptedFileNameBytes.Length);
                string encFileName;
                do
                {
                    encFileName = FileCastleUtil.GenerateRandomFileName();
                } while (File.Exists(encFileName));
                string fullPath = curFile.FullName.Replace(curFile.Name, encFileName);

                FileStream fsWrite = new FileStream(fullPath, FileMode.Append, FileAccess.Write);
                FileStream fsRead = new FileStream(curFile.FullName, FileMode.Open, FileAccess.Read);
                BufferedStream bsRead = new BufferedStream(fsRead, FileCastleConstants.BUFFER_SIZE_STANDARD);

                fsWrite.Write(rawBuffer, 0, rawBuffer.Length);
                Array.Clear(rawBuffer, 0, rawBuffer.Length);
                //int currentWriteFilePos = (int) encryptedFileNameBytes.Length;
                long totalBytesRead = 0;
                while (totalBytesRead < curFile.Length)
                {
                    int bytesRead = bsRead.Read(rawBuffer, 0, rawBuffer.Length);
                    if (bytesRead <= 0)
                    {
                        MessageBox.Show("bytesRead = " + bytesRead + "\n TotalBytesRead: " + totalBytesRead + "\n TotalFileSize: " + curFile.Length);
                        totalBytesRead = curFile.Length;
                        break;
                    }
                    totalBytesRead += bytesRead;
                    if (bytesRead != rawBuffer.Length)
                    {
                        byte[] swap = rawBuffer;
                        rawBuffer = new byte[bytesRead];
                        System.Buffer.BlockCopy(swap, 0, rawBuffer, 0, bytesRead);
                        //MessageBox.Show("Reached inside block! \n TotalBytesRead: " + totalBytesRead + "\n TotalFileSize: " + curFile.Length);
                    }
                    if (rawBuffer.Length > 0)
                    {
                        byte[] encryptedBytes = AES.Encrypt(rawBuffer, _key);
                        fsWrite.Write(encryptedBytes, 0, encryptedBytes.Length);
                    }
                    //currentWriteFilePos += encryptedBytes.Length;
                    totalProcessedBytes += bytesRead;
                    int progressPercent = (int)((totalProcessedBytes * 100) / _totalFileBytes);
                    if (progressPercent < 0)
                    {
                        MessageBox.Show("Negative progress percent!");
                    }
                    _progress.Report(progressPercent);
                    Array.Clear(rawBuffer, 0, rawBuffer.Length);
                }

                //MessageBox.Show("Encryption done");
                fsWrite.Close(); fsRead.Close(); bsRead.Close();
                //System.Buffer.BlockCopy(fileNameBytes, 0, rawBuffer, 0, fileNameBytes.Length);
                //long remainingFileSize = curFile.Length;
/*                while (remainingFileSize > 0)
                {
    
                }*/

                /*byte[] fileContentBytes = File.ReadAllBytes(curFile.FullName);

                // Append the bytes of the original filename with the bytes of the actual file content, separated by ":"
                byte[] fileNameBytes = ASCIIEncoding.ASCII.GetBytes(curFile.Name + ":");
                byte[] bytesToEncrypt = new byte[fileNameBytes.Length + fileContentBytes.Length];
                System.Buffer.BlockCopy(fileNameBytes, 0, bytesToEncrypt, 0, fileNameBytes.Length);
                System.Buffer.BlockCopy(fileContentBytes, 0, bytesToEncrypt, fileNameBytes.Length, fileContentBytes.Length);
                byte[] encryptedBytes = AES.Encrypt(bytesToEncrypt, key);
                string encFileName;
                do
                {
                    encFileName = FileCastleUtil.GenerateRandomFileName();
                } while (File.Exists(encFileName));
                string fullPath = curFile.FullName.Replace(curFile.Name, encFileName);
                File.WriteAllBytes(fullPath, encryptedBytes);*/

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
                if (File.Exists(Path.Combine(dir.FullName, FileCastleConstants.FC_META_INFO)))
                {
                    string encryptedDirName = File.ReadAllText(Path.Combine(dir.FullName, FileCastleConstants.FC_META_INFO));
                    string rawDirName = ASCIIEncoding.ASCII.GetString(AES.Decrypt(Convert.FromBase64String(encryptedDirName), key));
                    dir.MoveTo(Path.Combine(dir.Parent.FullName, rawDirName));
                    File.Delete(Path.Combine(dir.FullName, FileCastleConstants.FC_META_INFO));
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
                if (curFile.Extension == FileCastleConstants.ENCRYPTED_EXTENSION)
                {
                    byte[] encBuffer = new byte[FileCastleConstants.BUFFER_SIZE_STANDARD];
                    FileStream fsRead = new FileStream(fileName, FileMode.Open);
                    BufferedStream bsRead = new BufferedStream(fsRead);
                    bsRead.Read(encBuffer, 0, FileCastleConstants.BUFFER_SIZE_STANDARD);
                    byte[] fileNameLengthBytes = new byte[3];
                    System.Buffer.BlockCopy(encBuffer, 0, fileNameLengthBytes, 0, 3);
                    //string fileNameLengthStr = Encoding.ASCII.GetString(fileNameLengthBytes);
                    int fileNameLength = Convert.ToInt32(Encoding.ASCII.GetString(fileNameLengthBytes));
/*                    for (int i = 0; i < encBuffer.Length && encBuffer[i] != ':'; i++)
                    {
                        fileNameLength++;
                    }*/

                    byte[] encryptedFileNameBytes = new byte[fileNameLength];
                    System.Buffer.BlockCopy(encBuffer, 3, encryptedFileNameBytes, 0, fileNameLength);
                    byte[] decryptedFileNameBytes = AES.Decrypt(encryptedFileNameBytes, key);
                    string rawFileName = ASCIIEncoding.ASCII.GetString(decryptedFileNameBytes);
                    //byte[] remainingFileNameBytesBuffer = new byte[encBuffer.Length - fileNameLength + 1];
                    //System.Buffer.BlockCopy(encBuffer, fileNameLength + 1, remainingFileNameBytesBuffer, 0, remainingFileNameBytesBuffer.Length);
                    FileStream fsWrite = new FileStream(curFile.FullName.Replace(curFile.Name, rawFileName), FileMode.Append, FileAccess.Write);
                    //int len = encBuffer.Length - (fileNameLength + 1);
                    long totalBytesWritten = FileCastleConstants.BUFFER_SIZE_STANDARD;
                    Array.Clear(encBuffer, 0, encBuffer.Length);
                    encBuffer = new byte[FileCastleConstants.BUFFER_SIZE_PADDED];
                    while (totalBytesWritten < curFile.Length)
                    {
                        int bytesRead = bsRead.Read(encBuffer, 0, encBuffer.Length);
                        if (bytesRead <= 0)
                        {
                            MessageBox.Show("bytesRead = " + bytesRead + "\n TotalBytesRead: " + totalBytesWritten + "\n TotalFileSize: " + curFile.Length);
                            totalBytesWritten = curFile.Length;
                            break;
                        }
                        totalBytesWritten += bytesRead;
                        if (bytesRead != encBuffer.Length)
                        {
                            byte[] swap = encBuffer;
                            encBuffer = new byte[bytesRead];
                            System.Buffer.BlockCopy(swap, 0, encBuffer, 0, bytesRead);
                            //MessageBox.Show("Reached here!");
                        }
                        if (encBuffer.Length > 0)
                        {
                            byte[] decryptedBytes = AES.Decrypt(encBuffer, key);
                            fsWrite.Write(decryptedBytes, 0, decryptedBytes.Length);
                        }
                        else
                        {
                            MessageBox.Show("encBuffer < 0\nbytesRead = " + bytesRead + "\n TotalBytesRead: " + totalBytesWritten + "\n TotalFileSize: " + curFile.Length);
                        }

                        totalProcessedBytes += bytesRead;
                        int progressPercent = (int)((totalProcessedBytes * 100) / _totalFileBytes);
                        if (progressPercent < 0)
                        {
                            MessageBox.Show("Negative progress percent!");
                        }
                        _progress.Report(progressPercent);
                        //currentWriteFilePos += encryptedBytes.Length;
                        Array.Clear(encBuffer, 0, encBuffer.Length);
                    }
                    //MessageBox.Show("Finished writing.");
                    fsWrite.Close(); bsRead.Close(); fsRead.Close();
                    /*int fileNameLength = 0;
                    byte[] encryptedBytes = File.ReadAllBytes(fileName);
                    byte[] decryptedBytes = AES.Decrypt(encryptedBytes, key);
                    for (int i = 0; i < decryptedBytes.Length && decryptedBytes[i] != ':'; i++)
                    {
                        fileNameLength++;
                    }
                    byte[] fileNameBytes = new byte[fileNameLength];
                    byte[] fileContentBytes = new byte[decryptedBytes.Length - (fileNameLength + 1)];
                    System.Buffer.BlockCopy(decryptedBytes, 0, fileNameBytes, 0, fileNameLength);
                    System.Buffer.BlockCopy(decryptedBytes, fileNameLength + 1, fileContentBytes, 0, decryptedBytes.Length - (fileNameLength + 1));
                    string rawFileName = ASCIIEncoding.ASCII.GetString(fileNameBytes);
                    File.WriteAllBytes(curFile.FullName.Replace(curFile.Name, rawFileName), fileContentBytes);*/

                    // Delete encrypted file after successful decryption
                    File.Delete(fileName);
                }
            }
        }
        #endregion
    }
}
