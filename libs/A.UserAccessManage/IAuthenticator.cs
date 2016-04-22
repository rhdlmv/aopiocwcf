namespace A.UserAccessManage
{
    using A.UserAccessManage.Model;
    using System;

    public interface IAuthenticator
    {
        bool Authenticate(User user, string transactionCode, string credential, string extension);
    }
}

