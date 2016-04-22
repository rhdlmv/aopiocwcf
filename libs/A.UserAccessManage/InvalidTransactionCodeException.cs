namespace A.UserAccessManage
{
    using A.Commons.Exception;
    using System;

    public class InvalidTransactionCodeException : GenericException
    {
        public InvalidTransactionCodeException(Exception ex) : base(ex.Message, ex)
        {
        }

        public InvalidTransactionCodeException(string message) : base(message)
        {
        }

        public InvalidTransactionCodeException(string message, Exception ex) : base(message, ex)
        {
        }

        public override string ErrorCode
        {
            get
            {
                return "UE05-009";
            }
        }
    }
}

