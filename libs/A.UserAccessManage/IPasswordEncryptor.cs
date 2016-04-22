namespace A.UserAccessManage
{
    using A.UserAccessManage.Model;
    using System;

    public interface IPasswordEncryptor
    {
        string Decrypt(User user, string password);
        string Encrypt(User user, string password);

        bool CanDecrypt { get; }
    }
}

