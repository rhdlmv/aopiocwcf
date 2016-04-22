namespace AOPIOC.Wcf
{
    using System;

    public interface IExceptionFormatter
    {
        Exception Format(Exception exception);
    }
}

