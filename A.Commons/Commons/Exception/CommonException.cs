namespace EgoalTech.Commons.Exception
{
    using System;

    public class CommonException : GenericException
    {
        private string _errorCode;

        public CommonException(string errorCode, string message) : base(message)
        {
            this._errorCode = null;
            this._errorCode = errorCode;
        }

        public CommonException(string errorCode, string message, Exception ex) : base(message, ex)
        {
            this._errorCode = null;
            this._errorCode = errorCode;
        }

        public override string ErrorCode
        {
            get
            {
                return this._errorCode;
            }
        }
    }
}

