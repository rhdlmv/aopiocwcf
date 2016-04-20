namespace EgoalTech.Commons.Exception
{
    using System;

    public class ObjectAlreadyExistException : GenericException
    {
        private string errorCode;

        public ObjectAlreadyExistException(Type type, string message)
        {
            this.errorCode = "E00-006";
            base.AddExtraData("class", type.FullName);
            base._message = message;
        }

        public ObjectAlreadyExistException(Type type, string errorCode, string message)
        {
            this.errorCode = "E00-006";
            base.AddExtraData("class", type.FullName);
            base._message = message;
            this.errorCode = errorCode;
        }

        public override string ErrorCode =>
            this.errorCode
    }
}

