namespace A.UserAccessManage
{
    using A.Commons.Exception;
    using System;

    public class PasswordDecryptionException : GenericException
    {
        public PasswordDecryptionException(Exception ex) : base(ex.Message, ex)
        {
        }

        public PasswordDecryptionException(string message) : base(message)
        {
        }

        public PasswordDecryptionException(string message, Exception ex) : base(message, ex)
        {
        }

        public override string ErrorCode
        {
            get
            {
                return "UE05-012";
            }
        }
    }
}

