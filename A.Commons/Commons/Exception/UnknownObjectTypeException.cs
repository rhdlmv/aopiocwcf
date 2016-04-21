namespace EgoalTech.Commons.Exception
{
    using System;

    public class UnknownObjectTypeException : GenericException
    {
        public UnknownObjectTypeException(Exception ex) : base(ex.Message, ex)
        {
            base.SystemError = true;
        }

        public UnknownObjectTypeException(string message) : base(message)
        {
            base.SystemError = true;
        }

        public UnknownObjectTypeException(string message, Exception ex) : base(message, ex)
        {
            base.SystemError = true;
        }

        public override string ErrorCode
        {
            get
            {
                return "E00-003";
            }
        }
    }
}

