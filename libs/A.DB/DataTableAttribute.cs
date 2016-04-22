namespace A.DB
{
    using System;
    

    public class DataTableAttribute : Attribute
    {
        private string _tableName;

        public DataTableAttribute(string tableName)
        {
            this._tableName = tableName;
        }

        public string DatabaseName { get; set; }

        public string TableName
        {
            get
            {
                return this._tableName;
            }
        }
    }
}

