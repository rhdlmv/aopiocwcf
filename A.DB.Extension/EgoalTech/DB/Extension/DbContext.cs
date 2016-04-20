namespace EgoalTech.DB.Extension
{
    using EgoalTech.Commons.Exception;
    using EgoalTech.DB;
    using EgoalTech.Validation;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class DbContext : IDbContext, IDisposable
    {
        private EgoalTech.DB.Extension.ExpressionConverter _expressionConverter;

        public void BeginTransaction()
        {
            this.DbObjectOperator.BeginTransaction();
        }

        public void BeginTransaction(IsolationLevel level)
        {
            this.DbObjectOperator.BeginTransaction(level);
        }

        public void Commit()
        {
            this.DbObjectOperator.Commit();
        }

        public virtual T Create<T>(T model) where T: DbObject, new()
        {
            IDbObjectInfo<T> dbObjectInfo = this.DbRuleContext.GetDbObjectInfo<T>();
            model = dbObjectInfo.CreateClone(this, model);
            dbObjectInfo.CheckDuplicate(this, model, new string[0]);
            dbObjectInfo.SetModifyDatetime(model);
            this.Validate<T>(model, DbOperation.Create);
            this.DbObjectOperator.Insert(model);
            return model;
        }

        public void Delete<T>(Expression<Func<T, bool>> where) where T: DbObject, new()
        {
            List<T> list = this.QueryObjects<T>(where, 0x7fffffff, 0);
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    this.Delete<T>(list[i]);
                }
            }
        }

        public void Delete<T>(string id) where T: DbObject, new()
        {
            IDbObjectInfo<T> dbObjectInfo = this.DbRuleContext.GetDbObjectInfo<T>();
            DynamicPropertyInfo pkDynamicPropertyInfo = DbObjectTools.GetPkDynamicPropertyInfo(DbObjectTools.GetDbObjectInfo(typeof(T)), true);
            T model = this.QueryObject<T>(pkDynamicPropertyInfo.DataFieldName, id, true);
            dbObjectInfo.SetModifyDatetime(model);
            if (dbObjectInfo.IsLogicalDelete)
            {
                dbObjectInfo.SetLogicalDelete(model);
                this.DbObjectOperator.Update(model);
            }
            else
            {
                this.DbObjectOperator.Delete(model);
            }
        }

        public void Delete<T>(T model) where T: DbObject, new()
        {
            IDbObjectInfo<T> dbObjectInfo = this.DbRuleContext.GetDbObjectInfo<T>();
            if (dbObjectInfo.IsLogicalDelete)
            {
                T local = dbObjectInfo.RemoveClone(this, model);
                if (local != null)
                {
                    dbObjectInfo.SetModifyDatetime(local);
                    this.Validate<T>(model, DbOperation.None | DbOperation.Update);
                    this.DbObjectOperator.Update(local);
                }
            }
            else
            {
                dbObjectInfo.SetModifyDatetime(model);
                this.Validate<T>(model, DbOperation.Delete);
                this.DbObjectOperator.Delete(model);
            }
        }

        public void DeleteAll<T>() where T: DbObject, new()
        {
            this.DbObjectOperator.Delete<T>(null);
        }

        public void Dispose()
        {
            this.DbObjectOperator.Close();
        }

        public int ExecuteNonQuery(string sql) => 
            this.DbObjectOperator.ExecuteNonQuery(sql)

        public DataTable ExecuteQuery(string sql) => 
            this.DbObjectOperator.ExecuteQuery(sql)

        public F ExecuteScalar<T, F>(DbQuery<T> query) where T: DbObject, new()
        {
            string sql = query.ToSql(this.ExpressionConverter, new object[0]);
            return this.ExecuteScalar<F>(sql);
        }

        public T ExecuteScalar<T>(string sql) => 
            this.DbObjectOperator.ExecuteScalar<T>(sql)

        private OrderBy[] GetOrderBy<T>(OrderBy<T>[] orderBys) where T: DbObject, new()
        {
            List<OrderBy> list = new List<OrderBy>();
            if (orderBys != null)
            {
                foreach (OrderBy<T> by in orderBys)
                {
                    if ((by != null) && (by.Column != null))
                    {
                        OrderBy item = new OrderBy {
                            Column = this.ExpressionConverter.ToFieldName(by.Column),
                            Desc = by.Desc
                        };
                        list.Add(item);
                    }
                }
            }
            return list.ToArray();
        }

        private FieldInfo[] GetSystemPropertyFieldInfos<T>() where T: DbObject, new() => 
            (from field in typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                where field.IsDefined(typeof(DbParameterAttribute), true)
                select field).ToArray<FieldInfo>()

        private void InitParameter(string fullTableName, string name, object value)
        {
            string sql = $"select count(*) from {fullTableName} where name = '{name}'";
            if (this.ExecuteScalar<int>(sql) == 0)
            {
                string str2 = $"insert into {fullTableName}(id,name,value) values('{Guid.NewGuid().ToString()}','{name}','{value}')";
                this.ExecuteNonQuery(str2);
            }
        }

        private void InitParameters<T>() where T: DbObject, new()
        {
            T local = Activator.CreateInstance<T>();
            FieldInfo[] fields = DbObjectTools.GetFields(typeof(T));
            string tableName = DbObjectTools.GetTableName(typeof(T));
            foreach (FieldInfo info in fields)
            {
                DbParameterAttribute[] customAttributes = (DbParameterAttribute[]) info.GetCustomAttributes(typeof(DbParameterAttribute), true);
                if ((customAttributes != null) && (customAttributes.Length > 0))
                {
                    string name = info.Name;
                    object obj2 = info.GetValue(local) ?? "";
                    this.InitParameter(tableName, name, obj2);
                }
            }
        }

        public int QueryCount<T>(Expression<Func<T, bool>> where) where T: DbObject, new() => 
            this.QueryCount<T>(this.ExpressionConverter.ToCondition(where, new object[0]))

        public int QueryCount<T>(string condition) where T: DbObject, new()
        {
            condition = this.DbRuleContext.GetDbObjectInfo<T>().FixCondition(condition);
            return this.DbObjectOperator.QueryCount<T>(condition);
        }

        public virtual T QueryObject<T>(Expression<Func<T, bool>> where, bool checkExist = false) where T: DbObject, new() => 
            this.QueryObject<T>(this.ExpressionConverter.ToCondition(where, new object[0]), checkExist)

        public virtual T QueryObject<T>(string condition, bool checkExist = false) where T: DbObject, new()
        {
            T local = default(T);
            condition = this.DbRuleContext.GetDbObjectInfo<T>().FixCondition(condition);
            local = this.DbObjectOperator.Retrieve<T>(condition);
            if (checkExist && (local == null))
            {
                string format = "The {0} does not exist.";
                ObjectNotFoundException exception = new ObjectNotFoundException(typeof(T), string.Format(format, typeof(T).Name));
                exception.AddExtraData("QueryCondition", condition.Replace("[", "").Replace("]", "").Trim());
                throw exception;
            }
            return local;
        }

        public T QueryObject<T>(string key, object value, bool checkExist = false) where T: DbObject, new()
        {
            ISqlFormatter sqlFormatter = this.DbObjectOperator.SqlFormatter;
            string condition = $"{sqlFormatter.FormatFieldName(key)} = {sqlFormatter.FormatValue(value, false)}";
            return this.QueryObject<T>(condition, checkExist);
        }

        public List<T> QueryObjects<T>(Expression<Func<T, bool>> where, params OrderBy[] orderBy) where T: DbObject, new() => 
            this.QueryObjects<T>(this.ExpressionConverter.ToCondition(where, new object[0]), orderBy)

        public List<T> QueryObjects<T>(string condition, params OrderBy[] orderBys) where T: DbObject, new() => 
            this.QueryObjects<T>(condition, 0x7fffffff, 0, orderBys)

        public virtual List<T> QueryObjects<T>(Expression<Func<T, bool>> where, int rowCount, int pageIndex) where T: DbObject, new() => 
            this.QueryObjects<T>(this.ExpressionConverter.ToCondition(where, new object[0]), rowCount, pageIndex, new OrderBy[0])

        public virtual List<T> QueryObjects<T>(Expression<Func<T, bool>> where, int rowCount, int pageIndex, params OrderBy<T>[] orderBys) where T: DbObject, new()
        {
            OrderBy[] orderBy = this.GetOrderBy<T>(orderBys);
            return this.QueryObjects<T>(where, rowCount, pageIndex, orderBy);
        }

        public virtual List<T> QueryObjects<T>(Expression<Func<T, bool>> where, int rowCount, int pageIndex, params OrderBy[] orderBys) where T: DbObject, new() => 
            this.QueryObjects<T>(this.ExpressionConverter.ToCondition(where, new object[0]), rowCount, pageIndex, orderBys)

        public List<T> QueryObjects<T>(string condition, int rowCount, int pageIndex, params OrderBy[] orderBys) where T: DbObject, new()
        {
            condition = this.DbRuleContext.GetDbObjectInfo<T>().FixCondition(condition);
            return this.DbObjectOperator.QueryObjects<T>(condition, rowCount, pageIndex, orderBys);
        }

        public List<F> QueryObjects<F, T>(Expression<Func<T, bool>> where, Func<IDataReader, F> mapper, int rowCount, int pageIndex, params OrderBy[] orderBys) where T: DbObject, new() => 
            this.QueryObjects<F, T>(this.ExpressionConverter.ToCondition(where, new object[0]), mapper, rowCount, pageIndex, orderBys)

        public List<F> QueryObjects<F, T>(string condition, Func<IDataReader, F> mapper, int rowCount, int pageIndex, params OrderBy[] orderBys) where T: DbObject, new()
        {
            condition = this.DbRuleContext.GetDbObjectInfo<T>().FixCondition(condition);
            return this.DbObjectOperator.QueryObjects<F, T>(condition, mapper, rowCount, pageIndex, orderBys);
        }

        public T RetrieveObjectAsParameter<T>() where T: DbObject, new()
        {
            try
            {
                return this.DbObjectOperator.RetrieveObjectAsParamter<T>();
            }
            catch (FormatException)
            {
                this.InitParameters<T>();
                return this.DbObjectOperator.RetrieveObjectAsParamter<T>();
            }
        }

        public void Rollback()
        {
            this.DbObjectOperator.Rollback();
        }

        public void StoreObjectAsParameter<T>(T instance) where T: DbObject, new()
        {
            this.InitParameters<T>();
            this.DbObjectOperator.StoreObjectAsParameter(instance);
        }

        public virtual T Update<T>(T model) where T: DbObject, new()
        {
            IDbObjectInfo<T> dbObjectInfo = this.DbRuleContext.GetDbObjectInfo<T>();
            dbObjectInfo.CheckDuplicate(this, model, new string[0]);
            T local = dbObjectInfo.UpdateClone(this, model, new string[0]);
            this.Validate<T>(local, DbOperation.None | DbOperation.Update);
            dbObjectInfo.SetModifyDatetime(local);
            this.DbObjectOperator.Update(local);
            return local;
        }

        private bool Validate<T>(T obj, DbOperation dbOperation) where T: DbObject, new()
        {
            bool flag = true;
            if (this.DbRuleContext != null)
            {
                try
                {
                    flag = this.DbRuleContext.GetValidator<T>().Validate(obj, dbOperation);
                }
                catch (ObjectValidateException exception)
                {
                    DataVerifyErrorException exception2 = new DataVerifyErrorException(exception, true);
                    throw exception2;
                }
            }
            return flag;
        }

        public EgoalTech.DB.DbObjectOperator DbObjectOperator { get; set; }

        public IDbRuleContext DbRuleContext { get; set; }

        private EgoalTech.DB.Extension.ExpressionConverter ExpressionConverter
        {
            get
            {
                if (this._expressionConverter == null)
                {
                    this._expressionConverter = new EgoalTech.DB.Extension.ExpressionConverter(this.DbObjectOperator.SqlFormatter);
                }
                return this._expressionConverter;
            }
        }
    }
}

