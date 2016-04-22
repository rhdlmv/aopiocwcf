namespace A.DB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    public class SqlServerFormatter : ISqlFormatter
    {
        private static readonly string _quotePrefix = "[";
        private static readonly string _quoteSuffix = "]";
        private static readonly string _stringQuote = "'";
        private static readonly string _stringStrDecarator = "N";

        private void CheckIsView(DbObjectInfo dbObjectInfo)
        {
            if (!(!string.IsNullOrEmpty(dbObjectInfo.TableName) || string.IsNullOrEmpty(dbObjectInfo.QueryTable)))
            {
                throw new Exception("Cannot modify a view: " + dbObjectInfo.QueryTable);
            }
        }

        public virtual string FormatDatabaseName(string databaseName)
        {
            return (_quotePrefix + databaseName + _quoteSuffix);
        }

        public virtual string FormatFieldName(string fieldName)
        {
            return (_quotePrefix + fieldName + _quoteSuffix);
        }

        public virtual string FormatTableOrViewName(string tableOrViewName)
        {
            return (_quotePrefix + tableOrViewName + _quoteSuffix);
        }

        public virtual string FormatValue(object value, bool allowDBNull = false)
        {
            string str = ConvertUtils.ToSQL(value, allowDBNull);
            if (!allowDBNull || (value != null))
            {
                if (object.ReferenceEquals(value.GetType(), typeof(string)))
                {
                    return (_stringStrDecarator + _stringQuote + str + _stringQuote);
                }
                if (object.ReferenceEquals(value.GetType(), typeof(TimeSpan)) | object.ReferenceEquals(value.GetType(), typeof(DateTime)))
                {
                    return (_stringQuote + str + _stringQuote);
                }
            }
            return str;
        }

        public virtual string GetDeleteSQL(IStorageObject obj)
        {
            DbObjectInfo dbObjectInfo = DbObjectTools.GetDbObjectInfo(obj.GetType());
            this.CheckIsView(dbObjectInfo);
            DynamicPropertyInfo pkDynamicPropertyInfo = DbObjectTools.GetPkDynamicPropertyInfo(dbObjectInfo, true);
            object obj2 = obj.GetValue(pkDynamicPropertyInfo.PropertyName);
            string str = string.Format(" where {0} = {1}", this.FormatFieldName(pkDynamicPropertyInfo.DataFieldName), this.FormatValue(obj2, false));
            return string.Format(this.GetDeleteSqlTemplate(), this.FormatTableOrViewName(dbObjectInfo.TableName), str);
        }

        public virtual string GetDeleteSQL<T>(string condition) where T: IStorageObject
        {
            Type type = typeof(T);
            DbObjectInfo dbObjectInfo = DbObjectTools.GetDbObjectInfo(type);
            this.CheckIsView(dbObjectInfo);
            if (string.IsNullOrEmpty(condition))
            {
                condition = "";
            }
            else
            {
                condition = "where " + condition;
            }
            return string.Format(this.GetDeleteSqlTemplate(), this.FormatTableOrViewName(dbObjectInfo.TableName), condition);
        }

        public virtual string GetDeleteSqlTemplate()
        {
            return "delete from {0} {1}";
        }

        public virtual string GetInsertSQL(IStorageObject obj)
        {
            DbObjectInfo dbObjectInfo = DbObjectTools.GetDbObjectInfo(obj.GetType());
            this.CheckIsView(dbObjectInfo);
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            StringBuilder builder3 = new StringBuilder();
            DynamicPropertyInfo autoIncrementDynamicPropertyInfo = DbObjectTools.GetAutoIncrementDynamicPropertyInfo(dbObjectInfo);
            foreach (KeyValuePair<string, DynamicPropertyInfo> pair in dbObjectInfo.DynamicPropertyInfos)
            {
                string key = pair.Key;
                DynamicPropertyInfo info3 = pair.Value;
                if ((!info3.AutoIncrement && !info3.ReadOnly) && (info3.DefaultValue == null))
                {
                    object obj2 = obj.GetValue(info3.PropertyName);
                    string str2 = this.FormatFieldName(info3.DataFieldName);
                    if (builder3.Length > 0)
                    {
                        builder3.Append(",");
                    }
                    if (builder2.Length > 0)
                    {
                        builder2.Append(",");
                    }
                    builder3.Append(str2);
                    builder2.Append(this.FormatValue(obj2, info3.AllowDBNull));
                }
            }
            builder.AppendFormat(this.GetInsertSqlTemplate(), this.FormatTableOrViewName(dbObjectInfo.TableName), builder3.ToString(), builder2.ToString());
            if (autoIncrementDynamicPropertyInfo != null)
            {
                builder.Append("select CAST(SCOPE_IDENTITY() AS INT);");
            }
            return builder.ToString();
        }

        public virtual string GetInsertSqlTemplate()
        {
            return "insert into {0} ({1}) values ({2});";
        }

        public virtual string GetQueryCountSQL<T>(string condition = null) where T: IStorageObject
        {
            string str = "";
            Type type = typeof(T);
            DbObjectInfo dbObjectInfo = DbObjectTools.GetDbObjectInfo(type);
            if (!string.IsNullOrEmpty(condition))
            {
                str = string.Format(" where {0}", condition);
            }
            string tableOrViewName = dbObjectInfo.QueryTable ?? dbObjectInfo.TableName;
            return string.Format(this.GetSelectCountTemplate(), this.FormatTableOrViewName(tableOrViewName), str);
        }

        public virtual string GetQueryObjectsSQL<T>(string condition, int rowCount, int pageIndex, params OrderBy[] orderBys) where T: IStorageObject
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            StringBuilder builder3 = new StringBuilder();
            List<string> list = new List<string>();
            string str = "";
            Type type = typeof(T);
            DbObjectInfo dbObjectInfo = DbObjectTools.GetDbObjectInfo(type);
            int num = (rowCount == 0x7fffffff) ? 1 : ((pageIndex * rowCount) + 1);
            int num2 = 0x7fffffff;
            if ((rowCount != 0x7fffffff) && (rowCount != 0))
            {
                num2 = (num + rowCount) - 1;
            }
            if ((orderBys != null) && (orderBys.Length > 0))
            {
                builder.Append(" order by ");
                for (int i = 0; i < orderBys.Length; i++)
                {
                    if (i != 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(this.FormatFieldName(orderBys[i].Column));
                    if (orderBys[i].Desc)
                    {
                        builder.Append(" DESC");
                    }
                }
            }
            else
            {
                DynamicPropertyInfo pkDynamicPropertyInfo = DbObjectTools.GetPkDynamicPropertyInfo(dbObjectInfo, false);
                if (pkDynamicPropertyInfo != null)
                {
                    builder.AppendFormat(" order by {0}", this.FormatFieldName(pkDynamicPropertyInfo.DataFieldName));
                }
                else
                {
                    builder.Append(" order by Rand() ");
                }
            }
            foreach (KeyValuePair<string, DynamicPropertyInfo> pair in dbObjectInfo.DynamicPropertyInfos)
            {
                string key = pair.Key;
                DynamicPropertyInfo info3 = pair.Value;
                string str3 = this.FormatTableOrViewName(!string.IsNullOrEmpty(dbObjectInfo.QueryTable) ? dbObjectInfo.QueryTable : dbObjectInfo.TableName);
                string str4 = this.FormatFieldName(info3.DataFieldName);
                if (builder2.Length > 0)
                {
                    builder2.Append(",");
                }
                builder2.AppendFormat("{0}.{1}", str3, str4);
                if (!string.IsNullOrEmpty(info3.JoinTableName))
                {
                    string str5 = this.FormatTableOrViewName(dbObjectInfo.TableName);
                    string str6 = this.FormatFieldName(info3.DataFieldName);
                    string str7 = this.FormatTableOrViewName(info3.JoinTableName);
                    if (!string.IsNullOrEmpty(info3.JoinDatabaseName))
                    {
                        str7 = this.FormatDatabaseName(info3.JoinDatabaseName) + ".." + str7;
                    }
                    string str8 = this.FormatFieldName(info3.JoinOnFieldName);
                    string item = string.Format(" {0} {1} join {2} on {3} = {4} ", new object[] { info3.RightJoin ? "Right" : (info3.OuterJoin ? "Left" : ""), info3.OuterJoin ? "Outer" : "", str7, str7 + "." + str8, str5 + "." + str6 });
                    if (!list.Contains(item))
                    {
                        builder3.Append(item);
                        list.Add(item);
                    }
                }
            }
            string str10 = this.FormatTableOrViewName(!string.IsNullOrEmpty(dbObjectInfo.QueryTable) ? dbObjectInfo.QueryTable : dbObjectInfo.TableName);
            if (!string.IsNullOrEmpty(dbObjectInfo.DatabaseName))
            {
                str10 = this.FormatDatabaseName(dbObjectInfo.DatabaseName) + ".." + str10;
            }
            if (!string.IsNullOrEmpty(condition))
            {
                str = string.Format(" where {0}", condition);
            }
            return string.Format(this.GetSelectSqlWithPagingTemplate(), new object[] { builder2.ToString(), builder.ToString(), str10, builder3.ToString(), str, num, num2 });
        }

        public string GetSelectCountTemplate()
        {
            return "select count(*) from {0} {1} ";
        }

        public virtual string GetSelectSQL<T>(string condition) where T: IStorageObject
        {
            Type type = typeof(T);
            return this.GetSelectSQL(type, condition);
        }

        public virtual string GetSelectSQL(Type type, string condition)
        {
            DbObjectInfo dbObjectInfo = DbObjectTools.GetDbObjectInfo(type);
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, DynamicPropertyInfo> pair in dbObjectInfo.DynamicPropertyInfos)
            {
                string key = pair.Key;
                DynamicPropertyInfo info2 = pair.Value;
                string str2 = this.FormatTableOrViewName(!string.IsNullOrEmpty(dbObjectInfo.QueryTable) ? dbObjectInfo.QueryTable : dbObjectInfo.TableName);
                string str3 = this.FormatFieldName(info2.DataFieldName);
                if (builder.Length > 0)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("{0}.{1}", str2, str3);
                if (!string.IsNullOrEmpty(info2.JoinTableName))
                {
                    string tableName = dbObjectInfo.TableName;
                    string str5 = this.FormatFieldName(info2.DataFieldName);
                    string str6 = this.FormatTableOrViewName(info2.JoinTableName);
                    if (!string.IsNullOrEmpty(info2.JoinDatabaseName))
                    {
                        str6 = this.FormatDatabaseName(info2.JoinDatabaseName) + ".." + str6;
                    }
                    string str7 = this.FormatFieldName(info2.JoinOnFieldName);
                    string item = string.Format(" {0} {1} join {2} on {3} = {4} ", new object[] { info2.RightJoin ? "Right" : (info2.OuterJoin ? "Left" : ""), info2.OuterJoin ? "Outer" : "", str6, str6 + "." + str7, tableName + "." + str5 });
                    if (!list.Contains(item))
                    {
                        builder2.Append(item);
                        list.Add(item);
                    }
                }
            }
            string str9 = this.FormatTableOrViewName(!string.IsNullOrEmpty(dbObjectInfo.QueryTable) ? dbObjectInfo.QueryTable : dbObjectInfo.TableName);
            if (!string.IsNullOrEmpty(dbObjectInfo.DatabaseName))
            {
                str9 = this.FormatDatabaseName(dbObjectInfo.DatabaseName) + ".." + str9;
            }
            return string.Format(this.GetSelectSqlTemplate(), new object[] { builder.ToString(), str9, builder2.ToString(), condition });
        }

        public virtual string GetSelectSQL(Type type, string key, object value)
        {
            return this.GetSelectSQL(type, string.Format("{0} = {1}", key, this.FormatValue(value, false)));
        }

        public virtual string GetSelectSQLByKeyValue(Type type, object keyValue)
        {
            DynamicPropertyInfo pkDynamicPropertyInfo = DbObjectTools.GetPkDynamicPropertyInfo(DbObjectTools.GetDbObjectInfo(type), true);
            return this.GetSelectSQL(type, string.Format("{0} = {1}", this.FormatFieldName(pkDynamicPropertyInfo.DataFieldName), this.FormatValue(keyValue, false)));
        }

        public virtual string GetSelectSqlTemplate()
        {
            return "select {0} from {1} {2} where {3}";
        }

        public virtual string GetSelectSqlWithPagingTemplate()
        {
            return "select * from (select {0}, ROW_NUMBER() OVER ({1}) as row_number from {2} {3} {4}) as tb1 where row_number between {5} and {6}";
        }

        public virtual string GetSystemDateTimeSQL()
        {
            return "select getdate()";
        }

        public virtual string GetUpdateSQL(IStorageObject obj)
        {
            StringBuilder builder = new StringBuilder();
            DbObjectInfo dbObjectInfo = DbObjectTools.GetDbObjectInfo(obj.GetType());
            this.CheckIsView(dbObjectInfo);
            DynamicPropertyInfo pkDynamicPropertyInfo = DbObjectTools.GetPkDynamicPropertyInfo(dbObjectInfo, true);
            string[] source = obj.ModifiedValues.Keys.ToArray<string>();
            foreach (KeyValuePair<string, DynamicPropertyInfo> pair in dbObjectInfo.DynamicPropertyInfos)
            {
                string key = pair.Key;
                DynamicPropertyInfo info3 = pair.Value;
                if ((!info3.PrimaryKey && !info3.ReadOnly) && ((!info3.AutoIncrement && (info3.DefaultValue == null)) && source.Contains<string>(key)))
                {
                    object obj2 = obj.GetValue(info3.PropertyName);
                    string str2 = this.FormatFieldName(info3.DataFieldName);
                    if (builder.Length > 0)
                    {
                        builder.Append(",");
                    }
                    builder.AppendFormat("{0} = {1}", str2, this.FormatValue(obj2, info3.AllowDBNull));
                }
            }
            object obj3 = obj.GetValue(pkDynamicPropertyInfo.PropertyName);
            string str3 = this.FormatFieldName(pkDynamicPropertyInfo.DataFieldName) + " = " + this.FormatValue(obj3, false);
            return string.Format(this.GetUpdateSqlTemplate(), this.FormatTableOrViewName(dbObjectInfo.TableName), builder.ToString(), str3);
        }

        public virtual string GetUpdateSqlTemplate()
        {
            return "update {0} set {1} where {2}";
        }
    }
}

