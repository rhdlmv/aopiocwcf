namespace A.Validation
{
    using System;
    
    using System.Runtime.InteropServices;

    public class ValidateException : Exception
    {
        public ValidateException()
        {
        }

        public ValidateException(string message, object value = null) : base(message)
        {
            this.FieldValue = value;
        }

        public object FieldValue { get; private set; }
    }
}

