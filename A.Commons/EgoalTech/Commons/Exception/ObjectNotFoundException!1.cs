namespace EgoalTech.Commons.Exception
{
    using System;

    public class ObjectNotFoundException<T> : GenericException
    {
        private string errorCode;

        public ObjectNotFoundException(string errorCode, string message)
        {
            base.AddExtraData("class", typeof(T).FullName);
            base._message = message;
            this.errorCode = errorCode;
        }

        public override string ErrorCode =>
            this.errorCode
    }
}

