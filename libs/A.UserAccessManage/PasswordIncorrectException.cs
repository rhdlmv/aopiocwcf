namespace A.UserAccessManage
{
    using A.Commons.Exception;
    using System;

    public class PasswordIncorrectException : GenericException
    {
        public PasswordIncorrectException(Exception ex) : base(ex.Message, ex)
        {
        }

        public PasswordIncorrectException(string message) : base(message)
        {
        }

        public PasswordIncorrectException(string message, Exception ex) : base(message, ex)
        {
        }

        public override string ErrorCode
        {
            get
            {
                return "UE05-001";
            }
        }
    }
}

