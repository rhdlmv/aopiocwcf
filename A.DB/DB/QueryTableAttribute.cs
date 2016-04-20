namespace EgoalTech.DB
{
    using System;

    public class QueryTableAttribute : Attribute
    {
        public string _tableName;

        public QueryTableAttribute(string tableName)
        {
            this._tableName = tableName;
        }

        public string TableName
        {
            get
            {
                return this._tableName;
            }
        }
    }
}

