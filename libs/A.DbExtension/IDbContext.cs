using A.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
namespace A.DBExtension
{
    public interface IDbContext : IDisposable
    {
        void BeginTransaction();
        void BeginTransaction(IsolationLevel level);
        void Commit();
        T Create<T>(T model) where T : DbObject, new();
        void Delete<T>(Expression<Func<T, bool>> where) where T : DbObject, new();
        void Delete<T>(string id) where T : DbObject, new();
        void Delete<T>(T model) where T : DbObject, new();
        void DeleteAll<T>() where T : DbObject, new();
        int ExecuteNonQuery(string sql);
        DataTable ExecuteQuery(string sql);
        F ExecuteScalar<T, F>(DbQuery<T> query) where T : DbObject, new();
        T ExecuteScalar<T>(string sql);
        int QueryCount<T>(Expression<Func<T, bool>> where = null) where T : DbObject, new();
        int QueryCount<T>(string condition) where T : DbObject, new();
        T QueryObject<T>(Expression<Func<T, bool>> where, bool checkExist = false) where T : DbObject, new();
        T QueryObject<T>(string condition, bool checkExist = false) where T : DbObject, new();
        T QueryObject<T>(string key, object value, bool checkExist = false) where T : DbObject, new();
        List<T> QueryObjects<T>(Expression<Func<T, bool>> where, params OrderBy[] orderBy) where T : DbObject, new();
        List<T> QueryObjects<T>(string condition, params OrderBy[] orderBy) where T : DbObject, new();
        List<T> QueryObjects<T>(Expression<Func<T, bool>> where, int rowCount, int pageIndex) where T : DbObject, new();
        List<T> QueryObjects<T>(Expression<Func<T, bool>> where, int rowCount, int pageIndex, params OrderBy<T>[] orderBys) where T : DbObject, new();
        List<T> QueryObjects<T>(Expression<Func<T, bool>> where, int rowCount, int pageIndex, params OrderBy[] orderBys) where T : DbObject, new();
        List<T> QueryObjects<T>(string condition, int rowCount, int pageIndex, params OrderBy[] orderBy) where T : DbObject, new();
        List<F> QueryObjects<F, T>(Expression<Func<T, bool>> where, Func<IDataReader, F> mapper, int rowCount, int pageIndex, params OrderBy[] orderBys) where T : DbObject, new();
        List<F> QueryObjects<F, T>(string condition, Func<IDataReader, F> mapper, int rowCount, int pageIndex, params OrderBy[] orderBys) where T : DbObject, new();
        T RetrieveObjectAsParameter<T>() where T : DbObject, new();
        void Rollback();
        void StoreObjectAsParameter<T>(T instance) where T : DbObject, new();
        T Update<T>(T model) where T : DbObject, new();

        A.DB.DbObjectOperator DbObjectOperator { get; }

        IDbRuleContext DbRuleContext { get; }
    }
}

