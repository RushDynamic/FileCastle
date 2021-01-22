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
                _encryptedFilesCount++;
                DirectoryInfo dir = new DirectoryInfo(_fileName);
                var files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    VerifyCurrentFile(file.FullName, ref _encryptedFilesCount);
                }
            }
            else
            {
                if (new FileInfo(_fileName).Extension == ".castle")
                {
                    _encryptedFilesCount++;
                }
            }
        }
        #endregion

        #region "Encryption/Decryption"
        public void ProcessFiles(IProgress<int> _progress, List<string> _filesToProcess, string _key, Enums.Actions _ACTION)
        {
            //TODO: fix progressbar not getting updated
            int filesCount = 0, percent;
            foreach(string fileName in _filesToProcess)
            {
                if (_ACTION == Enums.Actions.Encrypt)
                    EncryptFile(fileName, _key);
                else
                    DecryptFile(fileName, _key);
                filesCount++;
                percent = (filesCount / _filesToProcess.Count) * 100;
                _progress.Report(percent);
            }
        }

        private void DecryptFile(string fileName, string key)
        {
            //TODO: Add directory check
            if (Directory.Exists(fileName))
            {
                DirectoryInfo dir = new DirectoryInfo(fileName);
                foreach (FileInfo file in dir.GetFiles())
                {
                    DecryptFile(file.FullName, key);
                }
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    DecryptFile(subDir.FullName, key);
                }
                string newDirName;
            }
            else
            {
                FileInfo curFile = new FileInfo(fileName);
                if (fileName.Contains(".castle"))
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
                    File.Delete(fileName);
                    File.WriteAllBytes(curFile.FullName.Replace(curFile.Name, rawFileName), fileContentBytes);
                }
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
                foreach(DirectoryInfo subDir in dir.GetDirectories())
                {
                    EncryptFile(subDir.FullName, key);
                }
                //TODO: Generate random directory names
                /*do
                {
                    newDirName = FileCastleUtil.GenerateRandomFileName();
                } while (Directory.Exists(newDirName));
                dir.MoveTo(Path.Combine(dir.Parent.FullName, newDirName));*/
            }
            else
            {
                FileInfo curFile = new FileInfo(fileName);
                byte[] fileContentBytes = File.ReadAllBytes(curFile.FullName);
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
                File.Delete(curFile.FullName);
                string fullPath = curFile.FullName.Replace(curFile.Name, encFileName);
                File.WriteAllBytes(fullPath, encryptedBytes);
            }
        }
        #endregion
    }
}
