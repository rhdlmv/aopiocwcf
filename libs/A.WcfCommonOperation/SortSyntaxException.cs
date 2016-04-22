namespace A.WcfCommonOperation
{
    using A.Commons.Exception;
    using System;

    public class SortSyntaxException : GenericException
    {
        public SortSyntaxException() : base("Sort syntax is not correct.")
        {
        }

        public SortSyntaxException(string message) : base(message)
        {
        }

        public override string ErrorCode
        {
            get
            {
                return "COSE01-002";
            }
        }
    }
}

