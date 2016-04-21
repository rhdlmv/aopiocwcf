namespace A.Validation
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class FieldValidateException : ValidateException
    {
        public FieldValidateException()
        {
        }

        public FieldValidateException(string message, string fieldName = null, object fieldvalue = null) : base(message, fieldvalue)
        {
            this.FieldName = fieldName;
        }

        public string FieldName { get; private set; }
    }
}

