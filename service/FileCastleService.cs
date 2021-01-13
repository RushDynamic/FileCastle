using FileCastle.constants;
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
    }
}
