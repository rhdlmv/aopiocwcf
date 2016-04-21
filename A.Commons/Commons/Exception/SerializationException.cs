namespace EgoalTech.Commons.Exception
{
    using System;

    public class SerializationException : GenericException
    {
        public SerializationException(Exception ex) : base(ex.Message, ex)
        {
            base.SystemError = true;
        }

        public SerializationException(string message) : base(message)
        {
            base.SystemError = true;
        }

        public SerializationException(string message, Exception ex) : base(message, ex)
        {
            base.SystemError = true;
        }

        public override string ErrorCode
        {
            get
            {
                return "E00-008";
            }
        }
    }
}

