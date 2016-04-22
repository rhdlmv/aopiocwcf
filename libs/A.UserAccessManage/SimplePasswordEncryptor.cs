namespace A.UserAccessManage
{
    using A.Commons.Utils;
    using A.UserAccessManage.Model;
    using System;

    public class SimplePasswordEncryptor : IPasswordEncryptor
    {
        public string Decrypt(User user, string password)
        {
            string str3;
            try
            {
                string key = EncryptUtils.GenSHA1Code(user.Username + user.Id);
                str3 = EncryptUtils.Decrypt(password, key);
            }
            catch (Exception exception)
            {
                throw new PasswordDecryptionException(exception);
            }
            return str3;
        }

        public string Encrypt(User user, string password)
        {
            string key = EncryptUtils.GenSHA1Code(user.Username + user.Id);
            return EncryptUtils.Encrypt(password, key);
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

