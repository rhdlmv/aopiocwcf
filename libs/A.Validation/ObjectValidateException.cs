namespace A.Validation
{
    using System;
    using System.Collections.Generic;
    
    using System.Runtime.InteropServices;
    using System.Text;

    public class ObjectValidateException : Exception
    {
        public ObjectValidateException()
        {
        }

        public ObjectValidateException(string message, string modelName = null, List<FieldValidateException> errors = null) : base(message)
        {
            this.ModelName = modelName;
            this.Errors = errors;
        }

        public string Format()
        {
            StringBuilder builder = new StringBuilder("\r\n");
            foreach (FieldValidateException exception in this.Errors)
            {
                object fieldValue = exception.FieldValue;
                if (fieldValue == null)
                {
                    fieldValue = "null";
                }
                else if (fieldValue is string)
                {
                    fieldValue = string.Format("\"{0}\"", exception.FieldValue.ToString().Replace("\"", "\\\""));
                }
                builder.AppendFormat("Error {0}({1}): {2} \r\n", exception.FieldName, fieldValue, exception.Message);
            }
            return builder.ToString();
        }

        public List<FieldValidateException> Errors { get; set; }

        public string ModelName { get; private set; }
    }
}

