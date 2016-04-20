namespace EgoalTech.DB
{
    using System;
    using System.Data;
    using System.Reflection;

    public class DbParameter
    {
        private ISqlFormatter _sqlFormatter;
        private DbOperator op;

        public DbParameter(DbOperator op)
        {
            this.op = op;
        }

        private string GetValue(string table, string name)
        {
            string str = "";
            string str2 = this.SqlFormatter.FormatFieldName("name");
            string sql = $"select * from {this.SqlFormatter.FormatTableOrViewName(table)} where {str2} = '{ConvertUtils.Escape(name)}'";
            IDataReader reader = this.op.Query(sql);
            if (reader.Read())
            {
                str = Convert.ToString(reader["value"]);
            }
            reader.Close();
            return str;
        }

        public void read(IStorageObject obj)
        {
            FieldInfo[] fields = DbObjectTools.GetFields(obj.GetType());
            string tableName = DbObjectTools.GetTableName(obj.GetType());
            int length = fields.Length;
            for (int i = 0; i < length; i++)
            {
                DbParameterAttribute[] customAttributes = (DbParameterAttribute[]) fields[i].GetCustomAttributes(typeof(DbParameterAttribute), true);
                if ((customAttributes != null) && (customAttributes.Length > 0))
                {
                    this.ReadField(tableName, fields[i], obj);
                }
            }
        }

        private void ReadField(string table, FieldInfo field, IStorageObject obj)
        {
            object obj2 = Convert.ChangeType(this.GetValue(table, field.Name), field.FieldType, null);
            field.SetValue(obj, obj2);
        }

        private void SetValue(string table, string name, string value)
        {
            string str = this.SqlFormatter.FormatFieldName("name");
            string str2 = this.SqlFormatter.FormatFieldName("value");
            string sql = $"update {this.SqlFormatter.FormatTableOrViewName(table)} set {str2} = '{ConvertUtils.Escape(value)}' where {str} = '{ConvertUtils.Escape(name)}'";
            this.op.ExecuteNonQuery(sql);
        }

        public void Write(IStorageObject obj)
        {
            FieldInfo[] fields = DbObjectTools.GetFields(obj.GetType());
            string tableName = DbObjectTools.GetTableName(obj.GetType());
            int length = fields.Length;
            for (int i = 0; i < length; i++)
            {
                DbParameterAttribute[] customAttributes = (DbParameterAttribute[]) fields[i].GetCustomAttributes(typeof(DbParameterAttribute), true);
                if ((customAttributes != null) && (customAttributes.Length > 0))
                {
                    this.WriteField(tableName, fields[i], obj);
                }
            }
        }

        private void WriteField(string table, FieldInfo field, IStorageObject obj)
        {
            string str;
            object obj2 = field.GetValue(obj);
            if (obj2.GetType().Equals(typeof(DateTime)))
            {
                str = ((DateTime) obj2).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                str = Convert.ToString(obj2);
            }
            this.SetValue(table, field.Name, str);
        }

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

