namespace A.DBExtension
{
    using System;
    

    public class SqlMethodAttribute : Attribute
    {
        public SqlMethodAttribute(string format)
        {
            this.Format = format;
        }

        public string Format { get; set; }
    }
}

