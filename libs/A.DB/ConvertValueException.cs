﻿using System;
namespace A.DB
{
    public class ConvertValueException : Exception
    {
        public ConvertValueException(string field, Exception ex) : base("column: " + field + ", error in convert value.", ex)
        {
        }
    }
}

