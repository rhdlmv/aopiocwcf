namespace A.UserAccessManage
{
    using A.Commons.Utils;
    using A.UserAccessManage.Model;
    using System;

    public class UsernamePasswordAuthenticator : IAuthenticator
    {
        public bool Authenticate(User user, string transactionCode, string credential, string extension)
        {
            return (EncryptUtils.GenSHA1Code(transactionCode + user.Password) == credential);
        }
    }
}

