namespace A.UserAccessManage
{
    using A.Commons.Exception;
    using System;

    public class AuthenticationException : GenericException
    {
        public AuthenticationException(Exception ex) : base(ex.Message, ex)
        {
        }

        public AuthenticationException(string message) : base(message)
        {
        }

        public AuthenticationException(string message, Exception ex) : base(message, ex)
        {
        }

        public override string ErrorCode
        {
            get
            {
                return "UE05-003";
            }
        }
    }
}

