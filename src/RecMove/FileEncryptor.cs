using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RecMove
{
    public static class FileEncryptor
    {
        /// <summary>
        /// 文字列からキー用のバイト配列を取得する
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static byte[] GetKeyBytes(string password)
        {
            var bytesPassword = Encoding.UTF8.GetBytes(password);
            var byteSalt = Encoding.UTF8.GetBytes("1PgLTBnoIL");
            using var stlHash = new Rfc2898DeriveBytes(bytesPassword, byteSalt, 84);
            return stlHash.GetBytes(16);
        }

        /// <summary>
        /// ファイルを暗号化する
        /// </summary>
        /// <param name="ifs"></param>
        /// <param name="ofs"></param>
        /// <param name="password"></param>
        public static void Encrypt(Stream ifs, Stream ofs, string password)
        {
            var keyBytes = FileEncryptor.GetKeyBytes(password);
            using var aes = new AesManaged()
            {
                BlockSize = 128,
                KeySize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = keyBytes,
            };

            aes.GenerateIV();
            ofs.Write(aes.IV, 0, aes.IV.Length);

            using var enc = aes.CreateEncryptor();
            using var cs = new CryptoStream(ofs, enc, CryptoStreamMode.Write, true);
            ifs.CopyTo(cs, 32 * 1024);
        }

        /// <summary>
        /// ファイルを復号化する
        /// </summary>
        /// <param name="ifs"></param>
        /// <param name="ofs"></param>
        /// <param name="password"></param>
        public static void Decrypt(Stream ifs, Stream ofs, string password)
        {
            var keyBytes = FileEncryptor.GetKeyBytes(password);
            using var aes = new AesManaged()
            {
                BlockSize = 128,
                KeySize = 128,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = keyBytes,
            };

            var ivBytes = new byte[aes.IV.Length];
            ifs.Read(ivBytes, 0, ivBytes.Length);
            aes.IV = ivBytes;

            /* Create AES Decryptor */
            using var encrypt = aes.CreateDecryptor();
            using var cs = new CryptoStream(ofs, encrypt, CryptoStreamMode.Write, true);
            ifs.CopyTo(cs, 32 * 1024);
        }
    }
}
