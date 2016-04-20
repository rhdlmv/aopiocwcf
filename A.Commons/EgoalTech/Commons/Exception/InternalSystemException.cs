namespace EgoalTech.Commons.Exception
{
    using System;

    public class InternalSystemException : GenericException
    {
        public InternalSystemException(Exception ex) : base(ex.Message, ex)
        {
            base.SystemError = true;
        }

        public InternalSystemException(string message) : base(message)
        {
            base.SystemError = true;
        }

        public InternalSystemException(string message, Exception ex) : base(message, ex)
        {
            base.SystemError = true;
        }

        public override string ErrorCode =>
            "E00-001"
    }
}

