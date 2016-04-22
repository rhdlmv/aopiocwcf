using A.Commons.Utils;
using System;
using System.Security.Cryptography;
using System.Text;

namespace A.UserAccessManage
{
    public class Utils
    {
        public static string DecodeValue(string value)
        {
            DES des = new DESCryptoServiceProvider();
            string s = "28566130";
            string str2 = "00000000";
            byte[] inputBuffer = StringUtils.ToByteArray(value);
            inputBuffer = des.CreateDecryptor(Encoding.ASCII.GetBytes(s), Encoding.ASCII.GetBytes(str2)).TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            return Encoding.ASCII.GetString(inputBuffer);
        }

        public static string EncodeValue(string value)
        {
            DES des = new DESCryptoServiceProvider();
            string s = "28566130";
            string str2 = "00000000";
            ICryptoTransform transform = des.CreateEncryptor(Encoding.ASCII.GetBytes(s), Encoding.ASCII.GetBytes(str2));
            byte[] bytes = Encoding.ASCII.GetBytes(value);
            return StringUtils.ToString(transform.TransformFinalBlock(bytes, 0, bytes.Length));
        }
    }
}

