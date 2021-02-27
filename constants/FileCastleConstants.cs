using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCastle.constants
{
    class FileCastleConstants
    {
        // UI
        public const string LABEL_HEADING_DROP_FILES = "DROP FILES/FOLDERS HERE";

        // Service
        public const string ENCRYPTED_EXTENSION = ".castle";
        public const string FC_META_INFO = ".fcMeta";
        public const Int32 BUFFER_SIZE_STANDARD = 8192;

        // AES Encryption method returns BUFFER_SIZE + 16 bytes as response
        public const Int32 BUFFER_SIZE_PADDED = 8208;
        public const Int32 FILE_NAME_LENGTH_CHUNK_SIZE = 3;
    }
}
