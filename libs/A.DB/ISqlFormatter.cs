namespace A.DB
{
    using System;
    using System.Runtime.InteropServices;

    public interface ISqlFormatter
    {
        string FormatDatabaseName(string databaseName);
        string FormatFieldName(string fieldName);
        string FormatTableOrViewName(string tableOrViewName);
        string FormatValue(object value, bool allowDBNull = false);
        string GetDeleteSQL(IStorageObject obj);
        string GetDeleteSQL<T>(string condition) where T: IStorageObject;
        string GetDeleteSqlTemplate();
        string GetInsertSQL(IStorageObject obj);
        string GetInsertSqlTemplate();
        string GetQueryCountSQL<T>(string condition = null) where T: IStorageObject;
        string GetQueryObjectsSQL<T>(string condition, int rowCount, int pageIndex, params OrderBy[] orderBys) where T: IStorageObject;
        string GetSelectCountTemplate();
        string GetSelectSQL<T>(string condition) where T: IStorageObject;
        string GetSelectSQL(Type type, string condition);
        string GetSelectSQL(Type type, string key, object value);
        string GetSelectSQLByKeyValue(Type type, object keyValue);
        string GetSelectSqlTemplate();
        string GetSelectSqlWithPagingTemplate();
        string GetSystemDateTimeSQL();
        string GetUpdateSQL(IStorageObject obj);
        string GetUpdateSqlTemplate();
    }
}

