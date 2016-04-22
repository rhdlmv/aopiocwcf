namespace A.DB
{
    using System;

    public class DbEventArgs : EventArgs
    {
        private DbOperationAction _action;
        private DbObjectOperator _operator;

        public DbEventArgs(DbObjectOperator op, DbOperationAction action)
        {
            this._operator = op;
            this._action = action;
        }

        public DbOperationAction Action
        {
            get
            {
                return this._action;
            }
        }

        public DbObjectOperator Operator
        {
            get
            {
                return this._operator;
            }
        }
    }
}

