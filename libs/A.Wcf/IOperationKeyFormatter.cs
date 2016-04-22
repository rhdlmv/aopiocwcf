namespace AOPIOC.Wcf
{
    using System;

    public interface IOperationKeyFormatter
    {
        string Format(OperationKey operationKey);
        OperationKey Parse(string value);
    }
}

