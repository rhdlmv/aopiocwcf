namespace EgoalTech.DB
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.Runtime.CompilerServices;

    public class DbOperator : IDisposable
    {
        private bool disposed;
        private bool exteneralConnection;
        private IDbTransaction trans;
        private EgoalTech.DB.DbType type;

        public DbOperator(IDbConnection connection)
        {
            this.disposed = false;
            this.Connection = connection;
            this.ConnectionString = connection.ConnectionString;
            this.exteneralConnection = true;
        }

        public DbOperator(string connestionString, System.Data.Common.DbProviderFactory dbProviderFactory)
        {
            this.disposed = false;
            this.ConnectionString = connestionString;
            this.DbProviderFactory = dbProviderFactory;
        }

        public void BeginTransaction()
        {
            if (this.trans == null)
            {
                this.Open();
                this.trans = this.Connection.BeginTransaction();
            }
        }

        public void BeginTransaction(IsolationLevel level)
        {
            if (this.trans == null)
            {
                this.Open();
                this.trans = this.Connection.BeginTransaction(level);
            }
        }

        public void Close()
        {
            this.TryClose();
        }

        public void Commit()
        {
            if (this.trans == null)
            {
                throw new Exception("Without Create Transaction");
            }
            this.trans.Commit();
            this.trans = null;
        }

        public IDbCommand CreateCommand()
        {
            this.Open();
            IDbCommand command = this.Connection.CreateCommand();
            command.Transaction = this.trans;
            return command;
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.TryClose();
                this.disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        public int ExecuteNonQuery(string sql)
        {
            IDbCommand command = this.CreateCommand();
            command.CommandText = sql;
            return command.ExecuteNonQuery();
        }

        public object ExecuteScalar(string sql)
        {
            IDbCommand command = this.CreateCommand();
            command.CommandText = sql;
            return command.ExecuteScalar();
        }

        public T ExecuteScalar<T>(string sql)
        {
            object obj2 = null;
            IDbCommand command = this.CreateCommand();
            command.CommandText = sql;
            obj2 = command.ExecuteScalar();
            if (obj2 == null)
            {
                return default(T);
            }
            Type type = typeof(T);
            return (T) ConvertUtils.ToObject(obj2, type, ConvertUtils.IsNullable(type));
        }

        public void Open()
        {
            if (!this.exteneralConnection && (this.Connection == null))
            {
                if (this.DbProviderFactory != null)
                {
                    this.Connection = this.DbProviderFactory.CreateConnection();
                    this.Connection.ConnectionString = this.ConnectionString;
                }
                else
                {
                    this.Connection = new OleDbConnection(this.ConnectionString);
                }
            }
            this.DbType = Database.parseDbType(this.ConnectionString);
            this.TryOpen();
        }

        public IDataReader Query(string sql)
        {
            IDbCommand command = this.CreateCommand();
            command.CommandText = sql;
            return command.ExecuteReader();
        }

        public DataTable QueryReturnWithDataTable(string sql)
        {
            IDbCommand command = this.CreateCommand();
            command.CommandText = sql;
            SqlDataAdapter adapter = new SqlDataAdapter((SqlCommand) command);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            return dataSet.Tables[0];
        }

        public void Rollback()
        {
            if (this.trans == null)
            {
                throw new Exception("Without Create Transaction");
            }
            this.trans.Rollback();
            this.trans = null;
        }

        private void TryClose()
        {
            if (!this.exteneralConnection && ((this.Connection == null) || !ConnectionState.Closed.Equals(this.Connection.State)))
            {
                try
                {
                    this.Connection.Close();
                }
                catch (Exception)
                {
                }
            }
        }

        private void TryConnection()
        {
            string str = "";
            if (this.type == EgoalTech.DB.DbType.ORACLE)
            {
                str = "select sysdate as dt from dual";
            }
            else if ((this.type == EgoalTech.DB.DbType.MSSQL) || (this.type == EgoalTech.DB.DbType.VISTADB))
            {
                str = "select GETDATE() as dt";
            }
            if (str.Length > 0)
            {
                IDbCommand command = this.CreateCommand();
                command.CommandText = str;
                command.ExecuteReader();
            }
        }

        private void TryOpen()
        {
            if ((this.Connection != null) && (this.Connection.State == ConnectionState.Open))
            {
                if (!this.TestConnection)
                {
                    return;
                }
                try
                {
                    this.TryConnection();
                    return;
                }
                catch (Exception)
                {
                }
            }
            try
            {
                this.Connection.Open();
            }
            catch
            {
            }
        }

        public IDbConnection Connection { get; private set; }

        public string ConnectionString { get; private set; }

        public System.Data.Common.DbProviderFactory DbProviderFactory { get; private set; }

        public EgoalTech.DB.DbType DbType { get; private set; }

        public bool IsBeginTransaction
        {
            get
            {
                return (this.trans != null);
            }
        }

        public bool TestConnection { get; set; }
    }
}

