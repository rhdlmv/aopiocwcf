namespace AOPIOC.Wcf
{
    using System;

    public class OperationKeyFormatter : IOperationKeyFormatter
    {
        public string Format(OperationKey operationKey)
        {
            return operationKey.SessionId;
        }

        public OperationKey Parse(string value)
        {
            return new OperationKey { SessionId = value };
        }
    }
}

