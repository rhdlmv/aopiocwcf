namespace A.UserAccessManage
{
    using A.Commons.Exception;
    using System;

    public class InvalidPolicyStateException : GenericException
    {
        public InvalidPolicyStateException(Exception ex) : base(ex.Message, ex)
        {
        }

        public InvalidPolicyStateException(string message) : base(message)
        {
        }

        public InvalidPolicyStateException(string message, Exception ex) : base(message, ex)
        {
        }

        public override string ErrorCode
        {
            get
            {
                return "UE05-008";
            }
        }
    }
}

