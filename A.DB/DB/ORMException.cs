namespace EgoalTech.DB
{
    using System;

    public class ORMException : Exception
    {
        public ORMException(string message) : base(message)
        {
        }
    }
}

