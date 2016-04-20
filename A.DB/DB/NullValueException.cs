namespace EgoalTech.DB
{
    using System;

    public class NullValueException : Exception
    {
        public NullValueException(string field) : base("column: " + field + ", null value is not allow.")
        {
        }
    }
}

