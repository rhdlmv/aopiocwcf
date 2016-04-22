namespace A.Commons.Exception
{
    using System;

    public class RemoveSystemDefaultRecordException<T> : GenericException
    {
        private string errorCode;

        public RemoveSystemDefaultRecordException(string errorCode, string message) : base(message)
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

