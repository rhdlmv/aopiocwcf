namespace EgoalTech.Commons.Exception
{
    using System;

    public class ObjectNotFoundException : GenericException
    {
        private string errorCode;

        public ObjectNotFoundException(Type type, string message)
        {
            this.errorCode = "E00-005";
            base.AddExtraData("class", type.FullName);
            base._message = message;
        }

        public ObjectNotFoundException(Type type, string errorCode, string message)
        {
            this.errorCode = "E00-005";
            base.AddExtraData("class", type.FullName);
            base._message = message;
            this.errorCode = errorCode;
        }

        public override string ErrorCode =>
            this.errorCode
    }
}

