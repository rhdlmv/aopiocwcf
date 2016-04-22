namespace A.UserAccessManage
{
    using A.UserAccessManage.Model;
    using System;

    public class PlainTextPasswordEncryptor : IPasswordEncryptor
    {
        public string Decrypt(User user, string password)
        {
            return password;
        }

        public string Encrypt(User user, string password)
        {
            return password;
        }

        public bool CanDecrypt
        {
            get
            {
                return true;
            }
        }
    }
}

