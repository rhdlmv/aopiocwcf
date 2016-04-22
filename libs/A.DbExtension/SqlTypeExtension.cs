using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Linq;

using System.Text;

namespace A.DBExtension
{
    public static class SqlTypeExtension
    {
        public static string FormatToSql(this object value)
        {
            if (value == null)
            {
                return "null";
            }
            if (value is bool)
            {
                return (value.Equals(true) ? "1" : "0");
            }
            if (value is Guid)
            {
                return ("'" + value.ToString() + "'");
            }
            if (value is string)
            {
                return ("N'" + value.ToString().Replace("'", "''") + "'");
            }
            if (value is DateTime)
            {
                DateTime time = (DateTime)value;
                if (time < SqlDateTime.MinValue.Value)
                {
                    time = SqlDateTime.MinValue.Value;
                }
                if (time == DateTime.MaxValue)
                {
                    time = DateTime.MaxValue.AddMilliseconds(-1.0);
                }
                return string.Format("'{0}'", time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            if (value is DateTimeOffset)
            {
                DateTimeOffset offset = (DateTimeOffset)value;
                if (offset < SqlDateTime.MinValue.Value)
                {
                    offset = SqlDateTime.MinValue.Value;
                }
                return string.Format("'{0}'", offset.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            if (value is TimeSpan)
            {
                TimeSpan span = (TimeSpan)value;
                return string.Format("'{0}:{1}:{2}.{3}'", new object[] { span.Hours, span.Minutes, span.Seconds, span.Milliseconds });
            }
            if (value is byte)
            {
                return string.Format("'{0}'", value);
            }
            if (((((value is short) || (value is int)) || ((value is long) || (value is float))) || (((value is double) || (value is decimal)) || ((value is ushort) || (value is uint)))) || (value is ulong))
            {
                return string.Format("{0}", value);
            }
            if (value is byte[])
            {
                byte[] inArray = value as byte[];
                return ("'" + Convert.ToBase64String(inArray) + "'");
            }
            Type type = value.GetType();
            IEnumerable enumerable = value as IEnumerable;
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                value = type.GetProperty("Value").GetValue(value, null);
                return value.FormatToSql();
            }
            if (enumerable == null)
            {
                throw new Exception(string.Format("The value {0} can't be parsed to SQL.", value));
            }
            StringBuilder builder = new StringBuilder("");
            foreach (object obj2 in enumerable)
            {
                string str = (obj2 == null) ? "null" : obj2.FormatToSql();
                builder.Append(str);
                builder.Append(",");
            }
            if (builder.Length == 0)
            {
                throw new Exception(string.Format("The enumer({0}) can't be parsed to SQL while it is empty.", value));
            }
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        public static bool IsSqlCondition(this Type type)
        {
            return type.GetInterfaces().Contains<Type>(typeof(DbQuery));
        }
    }
}

