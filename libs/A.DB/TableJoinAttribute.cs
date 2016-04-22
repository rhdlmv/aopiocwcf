namespace A.DB
{
    using System;

    public class TableJoinAttribute : Attribute
    {
        private TableJoinType _joinType;
        private string _leftFieldName;
        private string _rightFieldName;
        private string _tableName;

        public TableJoinAttribute(string tableName, string rightFieldName, string leftFieldName)
        {
            this._tableName = tableName;
            this._rightFieldName = rightFieldName;
            this._leftFieldName = leftFieldName;
            this._joinType = TableJoinType.InnerJoin;
        }

        public TableJoinType JoinType
        {
            get
            {
                return this._joinType;
            }
            set
            {
                this._joinType = value;
            }
        }

        public string LeftFieldName
        {
            get
            {
                return this._leftFieldName;
            }
        }

        public string RightFieldName
        {
            get
            {
                return this._rightFieldName;
            }
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

