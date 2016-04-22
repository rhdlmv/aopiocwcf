namespace A.UserAccessManage
{
    using A.Commons.Exception;
    using System;

    public class InvalidStatusException : GenericException
    {
        public InvalidStatusException(Exception ex) : base(ex.Message, ex)
        {
        }

        public InvalidStatusException(string message) : base(message)
        {
        }

        public InvalidStatusException(string message, Exception ex) : base(message, ex)
        {
        }

        public override string ErrorCode
        {
            get
            {
                return "UE05-010";
            }
        }
    }
}

