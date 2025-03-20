using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CodeWalker.GameFiles
{
    public class GTACrypto
    {
        public static byte[] DecryptAES(byte[] data)
        {
            return DecryptAESData(data, GTA5Keys.AES_KEY);
        }
        public static byte[] EncryptAES(byte[] data)
        {
            return EncryptAESData(data, GTA5Keys.AES_KEY);
        }
        public static byte[] DecryptAESData(byte[] data, byte[] key, int rounds = 1)
        {
            var rijndael = Rijndael.Create();
            rijndael.KeySize = 256;
            rijndael.Key = key;
            rijndael.BlockSize = 128;
            rijndael.Mode = CipherMode.ECB;
            rijndael.Padding = PaddingMode.None;

            var buffer = (byte[])data.Clone();
            var length = data.Length - data.Length % 16;

            // decrypt...
            if (length > 0)
            {
                var decryptor = rijndael.CreateDecryptor();
                for (var roundIndex = 0; roundIndex < rounds; roundIndex++)
                    decryptor.TransformBlock(buffer, 0, length, buffer, 0);
            }

            return buffer;
        }
        public static byte[] EncryptAESData(byte[] data, byte[] key, int rounds = 1)
        {
            var rijndael = Rijndael.Create();
            rijndael.KeySize = 256;
            rijndael.Key = key;
            rijndael.BlockSize = 128;
            rijndael.Mode = CipherMode.ECB;
            rijndael.Padding = PaddingMode.None;

            var buffer = (byte[])data.Clone();
            var length = data.Length - data.Length % 16;

            // encrypt...
            if (length > 0)
            {
                var encryptor = rijndael.CreateEncryptor();
                for (var roundIndex = 0; roundIndex < rounds; roundIndex++)
                    encryptor.TransformBlock(buffer, 0, length, buffer, 0);
            }

            return buffer;
        }

        public static byte[] GetTFITKey(string fileName, uint size)
        {
	        // Relevant C++ code (to get the key selector)
	        // unsigned int transformitSelector = 0;
	        // transformitSelector = atStringHash(ASSET.FileName(filename));
	        // transformitSelector += (int)m_ArchiveSize;
	        // transformitSelector = transformitSelector % TFIT_NUM_KEYS;

	        const int TFIT_NUM_KEYS = 0x65; // dec 101

	        uint transformitSelector = 0;
	        transformitSelector = RageStringHash.atStringHash(fileName);
	        transformitSelector += size; // archive size
	        transformitSelector %= TFIT_NUM_KEYS;

            return RageMultiKeys.unique_multikey_gta5_ps4[transformitSelector];
        }

        // PC uses different code for TFIT keys.
        // I've called this a 'Multi Key', as it uses multiple keys, but is otherwise just AES.
        // Used for Orbis and Durango (PS4 and XB1).
        public static byte[] DecryptMultiKey(byte[] data, string name, uint length)
        {
	        byte[] key = GetTFITKey(name, length);
	        return DecryptAESData(data, key);
        }

        public static byte[] EncryptMultiKey(byte[] data, string name, uint length)
        {
            byte[] key = GetTFITKey(name, length);
            return EncryptAESData(data, key);
        }
	}
}
