using FileCastle.constants;
using FileCastle.helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCastle.service
{
    class FileCastleService
    {
        // TODO: Add exception handling
        // TODO: Add more button functionality
        // TODO: Replace messages with constants
        // TODO: Cleanup filenames before writing to disk
        // TODO: Use compression for portability
        // TODO: Add comments for readability
        // TODO: Use LINQ wherever possible

        #region "File Extension Verification"
        public Enums.Actions VerifyAllFiles(List<string> _filesToConsider)
        {
            int filesCount = _filesToConsider.Count, encryptedFilesCount = 0;
            foreach (string fileName in _filesToConsider)
            {
                VerifyCurrentFile(fileName, ref encryptedFilesCount);
            }
            
            if ((encryptedFilesCount > 0) && (encryptedFilesCount >= filesCount))
            {
                return Enums.Actions.Decrypt;
            }
            else if (encryptedFilesCount == 0)
            {
                return Enums.Actions.Encrypt;
            }
            else
            {
                throw new Exception("Invalid selection of files.");
            }
        }

        private void VerifyCurrentFile(string _fileName, ref int _encryptedFilesCount)
        {
            if (Directory.Exists(_fileName))
            {
                DirectoryInfo dir = new DirectoryInfo(_fileName);
                var files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    VerifyCurrentFile(file.FullName, ref _encryptedFilesCount);
                }
            }
            else
            {
                if (new FileInfo(_fileName).Extension == FileCastleConstants.ENCRYPTED_EXTENSION)
                {
                    _encryptedFilesCount++;
                }
            }
        }
        #endregion

        #region "Encryption/Decryption"
        public void ProcessFiles(IProgress<int> _progress, List<string> _filesToProcess, string _key, Enums.Actions _ACTION)
        {
            int filesCount = 0, percent;
            foreach(string fileName in _filesToProcess)
            {
                if (_ACTION == Enums.Actions.Encrypt)
                    EncryptFile(fileName, _key);
                else
                    DecryptFile(fileName, _key);
                filesCount++;
                percent = (filesCount * 100) / _filesToProcess.Count;
                _progress.Report(percent);
            }
        }
        
        private void EncryptFile(string fileName, string key)
        {
            if (Directory.Exists(fileName))
            {
                DirectoryInfo dir = new DirectoryInfo(fileName);
                foreach(FileInfo file in dir.GetFiles())
                {
                    EncryptFile(file.FullName, key);
                }
                // Encrypt each file within sub-directories recursively
                foreach(DirectoryInfo subDir in dir.GetDirectories())
                {
                    EncryptFile(subDir.FullName, key);
                }

                // Generate a random directory that doesn't already exist
                string newDirName;
                do
                {
                    newDirName = FileCastleUtil.GenerateRandomFileName();
                } while (Directory.Exists(newDirName));

                // Write the original (encrypted) directory name into the fcMeta file
                File.WriteAllText(Path.Combine(dir.FullName, FileCastleConstants.FC_META_INFO), 
                    Convert.ToBase64String(AES.Encrypt(ASCIIEncoding.ASCII.GetBytes(fileName), key)));
                dir.MoveTo(Path.Combine(dir.Parent.FullName, newDirName));
            }
            else
            {
                FileInfo curFile = new FileInfo(fileName);
                byte[] fileContentBytes = File.ReadAllBytes(curFile.FullName);

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
                File.WriteAllBytes(fullPath, encryptedBytes);

                // Delete original file only after writing encrypted file to disk
                File.Delete(curFile.FullName);
            }
        }

        private void DecryptFile(string fileName, string key)
        {
            if (Directory.Exists(fileName))
            {
                DirectoryInfo dir = new DirectoryInfo(fileName);
                foreach (FileInfo file in dir.GetFiles())
                {
                    DecryptFile(file.FullName, key);
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
                    DecryptFile(subDir.FullName, key);
                }
            }
            else
            {
                FileInfo curFile = new FileInfo(fileName);
                if (curFile.Extension == FileCastleConstants.ENCRYPTED_EXTENSION)
                {
                    int fileNameLength = 0;
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
                    File.WriteAllBytes(curFile.FullName.Replace(curFile.Name, rawFileName), fileContentBytes);

                    // Delete encrypted file after successful decryption
                    File.Delete(fileName);
                }
            }
        }
        #endregion
    }
}
