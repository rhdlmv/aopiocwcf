namespace A.UserAccessManage
{
    using A.Commons.Exception;
    using System;

    public class DeactiveUserException : GenericException
    {
        public DeactiveUserException(Exception ex) : base(ex.Message, ex)
        {
        }

        public DeactiveUserException(string message) : base(message)
        {
        }

        public DeactiveUserException(string message, Exception ex) : base(message, ex)
        {
        }

        public override string ErrorCode
        {
            get
            {
                return "UE05-002";
            }
        }
    }
}

