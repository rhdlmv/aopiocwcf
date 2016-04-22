namespace A.Commons.Exception
{
    using System;

    public class ObjectDependencyException<T> : GenericException
    {
        private string errorCode;

        public ObjectDependencyException(string errorCode, string message)
        {
            base.AddExtraData("class", typeof(T).FullName);
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

