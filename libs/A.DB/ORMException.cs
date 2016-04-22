namespace A.DB
{
    using System;

    public class ORMException : Exception
    {
        public ORMException(string message) : base(message)
        {
        }
    }
}

