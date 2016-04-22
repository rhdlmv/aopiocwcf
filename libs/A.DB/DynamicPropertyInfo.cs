namespace A.DB
{
    using System;
    

    public class DynamicPropertyInfo
    {
        public bool AllowDBNull { get; set; }

        public bool AutoIncrement { get; set; }

        public string DataFieldName { get; set; }

        public object DefaultValue { get; set; }

        public string JoinDatabaseName { get; set; }

        public string JoinOnFieldName { get; set; }

        public string JoinTableName { get; set; }

        public int Length { get; set; }

        public int NumericPercision { get; set; }

        public int NumericScale { get; set; }

        public bool OuterJoin { get; set; }

        public bool PrimaryKey { get; set; }

        public string PropertyName { get; set; }

        public Type PropertyType { get; set; }

        public bool ReadOnly { get; set; }

        public bool RightJoin { get; set; }
    }
}

