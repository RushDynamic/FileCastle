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
            
            if ((encryptedFilesCount > 0) && (encryptedFilesCount == filesCount))
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
                if (new FileInfo(_fileName).Extension == ".castle")
                {
                    _encryptedFilesCount++;
                }
            }
        }
        #endregion

        #region "Encryption/Decryption"
        public void EncryptFiles(IProgress<int> _progress, List<string> _filesToEncrypt, string key)
        {
            //TODO: count of filesToEncrypt to calculate progress
            int filesCount = 0, percent;
            foreach(string fileName in _filesToEncrypt)
            {
                EncryptFile(fileName, key);
                filesCount++;
                percent = (filesCount / _filesToEncrypt.Count) * 100;
                _progress.Report(percent);
            }
        }

        public void DecryptFiles(IProgress<int> _progress)
        {
            //_progress.Report(15);
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
            }
            else
            {
                //TODO: Extract only filename from path, replace filename with random string, delete original file
                FileInfo curFile = new FileInfo(fileName);

                byte[] fileContentBytes = File.ReadAllBytes(curFile.FullName);
                byte[] fileNameBytes = ASCIIEncoding.ASCII.GetBytes(curFile.Name + ":");
                byte[] bytesToEncrypt = fileNameBytes.Concat(fileContentBytes).ToArray();
                File.Delete(curFile.FullName);
                byte[] encryptedBytes = AES.Encrypt(bytesToEncrypt, key);
                string encFileName = FileCastleUtil.GenerateRandomFileName();
                while(File.Exists(encFileName))
                {
                    encFileName = FileCastleUtil.GenerateRandomFileName();
                }
                string fullPath = curFile.FullName.Replace(curFile.Name, encFileName);
                File.WriteAllBytes(fullPath, encryptedBytes);
            }
        }
        #endregion
    }
}
