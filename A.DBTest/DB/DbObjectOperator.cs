namespace EgoalTech.DB
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class DbObjectOperator : IObjectOperator, IDisposable
    {
        private ISqlFormatter _sqlFormatter;
        private List<Action> transActions;

        public event EventHandler TranscationBegin;

        public event EventHandler TranscationCommit;

        public event EventHandler TranscationRollback;

        public DbObjectOperator()
        {
            this.Construct(Database.ConnectionString, null);
        }

        public DbObjectOperator(string connectionString)
        {
            this.Construct(connectionString, null);
        }

        public DbObjectOperator(string connectionString, DbProviderFactory dbProviderFactory)
        {
            this.Construct(connectionString, dbProviderFactory);
        }

        public void BeginTransaction()
        {
            this.DbOperator.BeginTransaction();
            this.transActions.Clear();
            this.IsTranscationInProgress = true;
            if (this.TranscationBegin != null)
            {
                this.TranscationBegin(this, EventArgs.Empty);
            }
        }

        public void BeginTransaction(IsolationLevel level)
        {
            this.DbOperator.BeginTransaction(level);
            this.transActions.Clear();
            this.IsTranscationInProgress = true;
            if (this.TranscationBegin != null)
            {
                this.TranscationBegin(this, EventArgs.Empty);
            }
        }

        public void Close()
        {
            if (this.DbOperator != null)
            {
                this.DbOperator.Close();
            }
        }

        public void Commit()
        {
            this.DbOperator.Commit();
            this.PerformActions();
            if (this.TranscationCommit != null)
            {
                this.TranscationCommit(this, EventArgs.Empty);
            }
            this.IsTranscationInProgress = false;
        }

        private void Construct(string connectionString, DbProviderFactory dbProviderFactory)
        {
            this.DbOperator = new EgoalTech.DB.DbOperator(connectionString, dbProviderFactory);
            this.transActions = new List<Action>();
        }

        public void Delete(IStorageObject obj)
        {
            string deleteSQL = this.SqlFormatter.GetDeleteSQL(obj);
            this.DbOperator.ExecuteNonQuery(deleteSQL);
            DbEventArgs e = new DbEventArgs(this, DbOperationAction.Delete);
            obj.OnWrote(this, e);
        }

        public void Delete<T>(string condition) where T: IStorageObject
        {
            string deleteSQL = this.SqlFormatter.GetDeleteSQL<T>(condition);
            this.DbOperator.ExecuteNonQuery(deleteSQL);
        }

        public void Dispose()
        {
            this.Close();
        }

        public int ExecuteNonQuery(string sql) => 
            this.DbOperator.ExecuteNonQuery(sql)

        public DataTable ExecuteQuery(string sql) => 
            this.DbOperator.QueryReturnWithDataTable(sql)

        public T ExecuteScalar<T>(string sql) => 
            this.DbOperator.ExecuteScalar<T>(sql)

        public DateTime GetServerDateTime()
        {
            string systemDateTimeSQL = this.SqlFormatter.GetSystemDateTimeSQL();
            return this.DbOperator.ExecuteScalar<DateTime>(systemDateTimeSQL);
        }

        public void Insert(IStorageObject obj)
        {
            DbEventArgs args;
            string insertSQL = this.SqlFormatter.GetInsertSQL(obj);
            Type type = obj.GetType();
            int num = this.DbOperator.ExecuteScalar<int>(insertSQL);
            DynamicPropertyInfo autoIncrementDynamicPropertyInfo = DbObjectTools.GetAutoIncrementDynamicPropertyInfo(DbObjectTools.GetDbObjectInfo(type));
            if ((autoIncrementDynamicPropertyInfo != null) & (num != 0))
            {
                obj.SetValue(autoIncrementDynamicPropertyInfo.PropertyName, num);
            }
            Action item = delegate {
                args = new DbEventArgs(this, DbOperationAction.Insert);
                obj.OnWrote(this, args);
            };
            if (this.DbOperator.IsBeginTransaction)
            {
                this.transActions.Add(item);
            }
            else
            {
                item();
            }
        }

        private void PerformActions()
        {
            int count = this.transActions.Count;
            for (int i = 0; i < count; i++)
            {
                this.transActions[i]();
            }
        }

        public int QueryCount<T>(string condition = null) where T: IStorageObject
        {
            string queryCountSQL = this.SqlFormatter.GetQueryCountSQL<T>(condition);
            Debug.WriteLine("GetQueryCountSQL: " + queryCountSQL);
            return this.DbOperator.ExecuteScalar<int>(queryCountSQL);
        }

        public List<T> QueryObjects<T>(string condition, int rowCount, int pageIndex, params OrderBy[] orderBys) where T: IStorageObject
        {
            List<T> list = new List<T>();
            Type type = typeof(T);
            string sql = this.SqlFormatter.GetQueryObjectsSQL<T>(condition, rowCount, pageIndex, orderBys);
            Debug.WriteLine("GetQueryObjectsSQL: " + sql);
            using (IDataReader reader = this.DbOperator.Query(sql))
            {
                while (reader.Read())
                {
                    T local = (T) Activator.CreateInstance(type);
                    this.Retrieve(local, reader);
                    list.Add(local);
                }
            }
            return list;
        }

        public List<F> QueryObjects<F, T>(string condition, Func<IDataReader, F> mapper, int rowCount, int pageIndex, params OrderBy[] orderBys) where T: IStorageObject
        {
            List<F> list = new List<F>();
            string sql = this.SqlFormatter.GetQueryObjectsSQL<T>(condition, rowCount, pageIndex, orderBys);
            using (IDataReader reader = this.DbOperator.Query(sql))
            {
                while (reader.Read())
                {
                    list.Add(mapper(reader));
                }
            }
            return list;
        }

        private void Read(IStorageObject obj, IDataReader reader)
        {
            if ((obj != null) && (reader != null))
            {
                Type type = obj.GetType();
                DbObjectInfo dbObjectInfo = DbObjectReflector.GetDbObjectInfo(type);
                if (dbObjectInfo == null)
                {
                    throw new Exception("Cannot retrieve DbObjectInfo of type : " + type.ToString());
                }
                foreach (KeyValuePair<string, DynamicPropertyInfo> pair in dbObjectInfo.DynamicPropertyInfos)
                {
                    string key = pair.Key;
                    DynamicPropertyInfo info2 = pair.Value;
                    try
                    {
                        ConvertUtils.SetValueToObject(reader[info2.DataFieldName], obj, key, info2.PropertyType);
                    }
                    catch (Exception exception)
                    {
                        throw new ConvertValueException(info2.DataFieldName, exception);
                    }
                }
            }
        }

        public T Retrieve<T>(object keyValue) where T: IStorageObject, new()
        {
            T local = default(T);
            Type type = typeof(T);
            string selectSQLByKeyValue = this.SqlFormatter.GetSelectSQLByKeyValue(type, keyValue);
            Debug.WriteLine("SelectSQL: " + selectSQLByKeyValue);
            using (IDataReader reader = this.DbOperator.Query(selectSQLByKeyValue))
            {
                if (reader.Read())
                {
                    local = (T) Activator.CreateInstance(type);
                    this.Read(local, reader);
                    DbEventArgs e = new DbEventArgs(this, DbOperationAction.Select);
                    local.OnRead(this, e);
                }
            }
            return local;
        }

        public T Retrieve<T>(string condition) where T: IStorageObject, new()
        {
            T local = default(T);
            Type type = typeof(T);
            string selectSQL = this.SqlFormatter.GetSelectSQL<T>(condition);
            Debug.WriteLine("SelectSQL: " + selectSQL);
            using (IDataReader reader = this.DbOperator.Query(selectSQL))
            {
                if (reader.Read())
                {
                    local = (T) Activator.CreateInstance(type);
                    this.Read(local, reader);
                    DbEventArgs e = new DbEventArgs(this, DbOperationAction.Select);
                    local.OnRead(this, e);
                }
            }
            return local;
        }

        public void Retrieve(IStorageObject obj, IDataReader reader)
        {
            this.Read(obj, reader);
            DbEventArgs e = new DbEventArgs(this, DbOperationAction.Select);
            obj.OnRead(this, e);
        }

        public bool Retrieve(IStorageObject obj, string condition)
        {
            bool flag = false;
            string selectSQL = this.SqlFormatter.GetSelectSQL(obj.GetType(), condition);
            Debug.WriteLine("SelectSQL: " + selectSQL);
            using (IDataReader reader = this.DbOperator.Query(selectSQL))
            {
                if (reader.Read())
                {
                    this.Read(obj, reader);
                    DbEventArgs e = new DbEventArgs(this, DbOperationAction.Select);
                    obj.OnRead(this, e);
                    flag = true;
                }
            }
            return flag;
        }

        public T Retrieve<T>(string keyName, object keyValue) where T: IStorageObject, new()
        {
            T local3 = default(T);
            T local = (local3 == null) ? Activator.CreateInstance<T>() : (local3 = default(T));
            if (!this.Retrieve(local, keyName, keyValue))
            {
                return default(T);
            }
            return local;
        }

        public bool Retrieve(IStorageObject obj, string keyName, object keyValue)
        {
            bool flag = false;
            string sql = this.SqlFormatter.GetSelectSQL(obj.GetType(), keyName, keyValue);
            Debug.WriteLine("SelectSQL: " + sql);
            using (IDataReader reader = this.DbOperator.Query(sql))
            {
                if (reader.Read())
                {
                    this.Read(obj, reader);
                    DbEventArgs e = new DbEventArgs(this, DbOperationAction.Select);
                    obj.OnRead(this, e);
                    flag = true;
                }
            }
            return flag;
        }

        public bool RetrieveByKey(IStorageObject obj)
        {
            DynamicPropertyInfo pkDynamicPropertyInfo = DbObjectTools.GetPkDynamicPropertyInfo(DbObjectReflector.GetDbObjectInfo(obj.GetType()), true);
            if (pkDynamicPropertyInfo == null)
            {
                throw new ArgumentException("Primary Key Not Found");
            }
            object keyValue = obj.GetValue(pkDynamicPropertyInfo.PropertyName);
            return this.Retrieve(obj, pkDynamicPropertyInfo.DataFieldName, keyValue);
        }

        public bool RetrieveByKey(IStorageObject obj, object keyValue)
        {
            DynamicPropertyInfo pkDynamicPropertyInfo = DbObjectTools.GetPkDynamicPropertyInfo(DbObjectReflector.GetDbObjectInfo(obj.GetType()), true);
            if (pkDynamicPropertyInfo == null)
            {
                throw new ArgumentException("Primary Key Not Found");
            }
            return this.Retrieve(obj, pkDynamicPropertyInfo.DataFieldName, keyValue);
        }

        public T RetrieveObjectAsParamter<T>() where T: IStorageObject, new()
        {
            EgoalTech.DB.DbParameter parameter = new EgoalTech.DB.DbParameter(this.DbOperator);
            T local = new T();
            parameter.read(local);
            return local;
        }

        public void Rollback()
        {
            this.DbOperator.Rollback();
            this.transActions.Clear();
            if (this.TranscationRollback != null)
            {
                this.TranscationRollback(this, EventArgs.Empty);
            }
            this.IsTranscationInProgress = false;
        }

        public void StoreObjectAsParameter(IStorageObject obj)
        {
            new EgoalTech.DB.DbParameter(this.DbOperator).Write(obj);
        }

        public void Update(IStorageObject obj)
        {
            DbEventArgs args;
            if (obj.ModifiedValues.Count != 0)
            {
                string[] strArray = obj.ModifiedValues.Keys.ToArray<string>();
                string updateSQL = this.SqlFormatter.GetUpdateSQL(obj);
                Debug.WriteLine("GetUpdateSQL: " + updateSQL);
                this.DbOperator.ExecuteNonQuery(updateSQL);
                Action item = delegate {
                    args = new DbEventArgs(this, DbOperationAction.Update);
                    obj.OnWrote(this, args);
                };
                if (this.DbOperator.IsBeginTransaction)
                {
                    this.transActions.Add(item);
                }
                else
                {
                    item();
                }
            }
        }

        public EgoalTech.DB.DbOperator DbOperator { get; private set; }

        public bool IsTranscationInProgress { get; private set; }

        public ISqlFormatter SqlFormatter
        {
            get
            {
                if (this._sqlFormatter == null)
                {
                    this._sqlFormatter = new SqlServerFormatter();
                }
                return this._sqlFormatter;
            }
            set
            {
                if (value != null)
                {
                    this._sqlFormatter = value;
                }
            }
        }
    }
}

