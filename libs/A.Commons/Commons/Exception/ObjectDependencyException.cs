namespace A.Commons.Exception
{
    using System;

    public class ObjectDependencyException : GenericException
    {
        private string errorCode;

        public ObjectDependencyException(Type type, string errorCode, string message)
        {
            base.AddExtraData("class", type.FullName);
            base._message = message;
            this.errorCode = errorCode;
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

