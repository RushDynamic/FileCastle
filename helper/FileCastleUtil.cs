using FileCastle.constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCastle.helper
{
    class FileCastleUtil
    {
        public static string GenerateRandomFileName()
        {
            using (System.Security.Cryptography.RNGCryptoServiceProvider crypto = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[32];
                crypto.GetNonZeroBytes(buffer);
                string rawGeneratedName = String.Format("{0}{1}",Convert.ToBase64String(buffer), FileCastleConstants.ENCRYPTED_EXTENSION);
                return CleanFileName(rawGeneratedName);
            }
        }

        public static string CleanFileName(string fileName)
        {
            return String.Concat(fileName.Where(c => !System.IO.Path.GetInvalidFileNameChars().Contains(c)));
        }
    }
}
