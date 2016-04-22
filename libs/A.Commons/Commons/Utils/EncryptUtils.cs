namespace A.Commons.Utils
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class EncryptUtils
    {
        public static string ComputeHash(string input, HashAlgorithm algorithm)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] buffer2 = algorithm.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < buffer2.Length; i++)
            {
                builder.Append(buffer2[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        public static string Decrypt(string encryptedText, string key)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(Decrypt(encryptedData, GetRijndaelManaged(key)));
        }

        public static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        public static string Encrypt(string plainText, string key)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(plainText), GetRijndaelManaged(key)));
        }

        public static string GenMd5Code(string plaintext)
        {
            MD5 algorithm = MD5.Create();
            return ComputeHash(plaintext, algorithm);
        }

        public static string GenSHA1Code(string plaintext)
        {
            SHA1 algorithm = SHA1.Create();
            return ComputeHash(plaintext, algorithm);
        }

        public static RijndaelManaged GetRijndaelManaged(string secretKey)
        {
            byte[] destinationArray = new byte[0x10];
            byte[] bytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(bytes, destinationArray, Math.Min(destinationArray.Length, bytes.Length));
            return new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7, KeySize = 0x80, BlockSize = 0x80, Key = destinationArray, IV = destinationArray };
        }
    }
}

