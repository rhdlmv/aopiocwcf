namespace A.WcfCommonOperation
{
    using A.Commons.Exception;
    using System;

    public class QuerySyntaxException : GenericException
    {
        public QuerySyntaxException() : base("Query syntax is not correct.")
        {
        }

        public QuerySyntaxException(string message) : base(message)
        {
        }

        public override string ErrorCode
        {
            get
            {
                return "COSE01-001";
            }
        }
    }
}

