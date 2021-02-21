using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCastle.constants
{
    class FileCastleConstants
    {
        public const string ENCRYPTED_EXTENSION = ".castle";
        public const string FC_META_INFO = ".fcMeta";
        public const Int32 BUFFER_SIZE_STANDARD = 8192;

        // AES Encryption method returns BUFFER_SIZE + 16 Bytes as response
        public const Int32 BUFFER_SIZE_PADDED = 8208;
    }
}
