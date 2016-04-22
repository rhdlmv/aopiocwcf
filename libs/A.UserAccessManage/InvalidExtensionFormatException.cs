namespace A.UserAccessManage
{
    using A.Commons.Exception;
    using System;

    public class InvalidExtensionFormatException : GenericException
    {
        public InvalidExtensionFormatException(Exception ex) : base(ex.Message, ex)
        {
        }

        public InvalidExtensionFormatException(string message) : base(message)
        {
        }

        public InvalidExtensionFormatException(string message, Exception ex) : base(message, ex)
        {
        }

        public override string ErrorCode
        {
            get
            {
                return "UE05-011";
            }
        }
    }
}

