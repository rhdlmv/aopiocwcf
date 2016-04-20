namespace EgoalTech.Commons.Exception
{
    using System;

    public class ObjectAlreadyExistException<T> : GenericException
    {
        private string errorCode;

        public ObjectAlreadyExistException(string errorCode, string message)
        {
            base.AddExtraData("class", typeof(T).FullName);
            base._message = message;
            this.errorCode = errorCode;
        }

        public override string ErrorCode =>
            this.errorCode
    }
}

