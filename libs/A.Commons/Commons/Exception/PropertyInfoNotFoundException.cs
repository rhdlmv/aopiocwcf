namespace A.Commons.Exception
{
    using System;

    public class PropertyInfoNotFoundException : GenericException
    {
        public PropertyInfoNotFoundException(Exception ex) : base(ex.Message, ex)
        {
            base.SystemError = true;
        }

        public PropertyInfoNotFoundException(string message) : base(message)
        {
            base.SystemError = true;
        }

        public PropertyInfoNotFoundException(string message, Exception ex) : base(message, ex)
        {
            base.SystemError = true;
        }

        public override string ErrorCode
        {
            get
            {
                return "E00-004";
            }
        }
    }
}

