namespace A.UserAccessManage
{
    using A.Commons.Utils;
    using A.UserAccessManage.Model;
    using System;

    public class SHA1PasswordEncryptor : IPasswordEncryptor
    {
        public string Decrypt(User user, string password)
        {
            throw new NotImplementedException();
        }

        public string Encrypt(User user, string password)
        {
            if (password == null)
            {
                return password;
            }
            return EncryptUtils.GenSHA1Code(password);
        }

        public bool CanDecrypt
        {
            get
            {
                return false;
            }
        }
    }
}

