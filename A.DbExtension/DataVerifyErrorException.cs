namespace A.DBExtension
{
    using EgoalTech.Commons.Exception;
    using A.Validation;
    using System;
    using System.Text;

    public class DataVerifyErrorException : GenericException
    {
        public DataVerifyErrorException(ObjectValidateException ex, bool showErrorValue) : base(string.Format("The validation of instance ({0}) is failed.", ex.get_ModelName()) + FormatException(ex, showErrorValue), ex)
        {
        }

        public static string FormatException(ObjectValidateException ex, bool showErrorValue)
        {
            StringBuilder builder = new StringBuilder();
            foreach (FieldValidateException exception in ex.get_Errors())
            {
                string format = "{0}: {1}";
                if (showErrorValue)
                {
                    format = format + "{0}: can not be {2}.{1}";
                }
                object obj2 = exception.get_FieldValue();
                if (obj2 == null)
                {
                    obj2 = "null";
                }
                else if (obj2 is string)
                {
                    obj2 = string.Format("\"{0}\"", exception.get_FieldValue().ToString().Replace("\"", "\\\""));
                }
                builder.AppendFormat(format, exception.get_FieldName(), exception.Message, obj2);
            }
            return builder.ToString();
        }

        public override string ErrorCode
        {
            get
            {
                return "DBE00-002";
            }
        }
    }
}

