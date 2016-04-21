namespace EgoalTech.Commons.Exception
{
    using System;

    public class FailedToSendEmailException : GenericException
    {
        public string errorCode;

        public FailedToSendEmailException() : base("Failed to send email.")
        {
            this.errorCode = "E00-009";
        }

        public FailedToSendEmailException(Exception ex) : base(ex)
        {
            this.errorCode = "E00-009";
        }

        public FailedToSendEmailException(string message) : base(message)
        {
            this.errorCode = "E00-009";
        }

        public FailedToSendEmailException(string message, Exception ex) : base(message, ex)
        {
            this.errorCode = "E00-009";
        }

        public override string ErrorCode
        {
            get
            {
                return this.errorCode;
            }
        }
    }
}

