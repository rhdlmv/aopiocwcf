namespace EgoalTech.DB
{
    using System;

    public class DataSchemaAttribute : Attribute
    {
        private string _schemaName;

        public DataSchemaAttribute(string schemaName)
        {
            this._schemaName = schemaName;
        }

        public string SchemaName
        {
            get => 
                this._schemaName
            set
            {
                this._schemaName = value;
            }
        }
    }
}

