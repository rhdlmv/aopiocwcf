namespace EgoalTech.Commons.Exception
{
    using System;

    public class ArgumentIsNullOrEmptyException : GenericException
    {
        public ArgumentIsNullOrEmptyException(Exception ex) : base(ex.Message, ex)
        {
        }

        public ArgumentIsNullOrEmptyException(string message) : base(message)
        {
        }

        public ArgumentIsNullOrEmptyException(string message, Exception ex) : base(message, ex)
        {
        }

        public override string ErrorCode =>
            "E00-002"
    }
}

