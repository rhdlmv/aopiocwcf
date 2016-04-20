namespace AOPIOC.Wcf
{
    using EgoalTech.DB;
    using System;
    using System.Data;
    using System.Diagnostics;

    public class TransactionAttribute : OperationAttribute
    {
        private string connectionName;
        private IsolationLevel level;

        public TransactionAttribute() : this(null, IsolationLevel.ReadCommitted)
        {
        }

        public TransactionAttribute(IsolationLevel level) : this(null, level)
        {
        }

        public TransactionAttribute(string connectionName, IsolationLevel level)
        {
            this.connectionName = connectionName;
            this.level = level;
        }

        public override void AfterInvoke(object instance, object[] inputs, object result)
        {
            DbObjectOperator dbObjectOperator = this.GetDbObjectOperator(this.connectionName);
            Debug.WriteLine("Commit transaction hash: " + dbObjectOperator.GetHashCode());
            dbObjectOperator.Commit();
        }

        public override void BeforeInvoke(object instance, object[] inputs)
        {
            Debug.WriteLine("Transaction");
            DbObjectOperator dbObjectOperator = null;
            dbObjectOperator = this.GetDbObjectOperator(this.connectionName);
            dbObjectOperator.BeginTransaction(this.level);
            Debug.WriteLine("Begin transaction hash: " + dbObjectOperator.GetHashCode());
        }

        private DbObjectOperator GetDbObjectOperator(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
            {
                return WcfContext.Current.GetDbObjectOperator();
            }
            return WcfContext.Current.GetDbObjectOperator(connectionName);
        }

        public override void OnInvokeError(object instance, object[] inputs, object result)
        {
            try
            {
                this.GetDbObjectOperator(this.connectionName).Rollback();
            }
            catch
            {
            }
        }
    }
}

