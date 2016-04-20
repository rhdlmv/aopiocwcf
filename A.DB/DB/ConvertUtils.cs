namespace EgoalTech.DB
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class ConvertUtils
    {
        public static string Escape(string str)
        {
            if (str == null)
            {
                return "";
            }
            return str.Replace("'", "''");
        }

        public static string GetDateString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public static string GetDateTimeOffsetString(DateTimeOffset date)
        {
            return string.Format("'{0}'", date.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        public static bool IsNullable(Type type)
        {
            return (!(type.IsValueType || type.IsClass) || ((type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>))) || type.Equals(typeof(string))));
        }

        private static void SetNullableTypeValue(object obj, object val)
        {
            PropertyInfo property = obj.GetType().GetProperty("Value");
            if (property == null)
            {
                throw new ArgumentException("Null Property");
            }
            property.SetValue(obj, val, null);
        }

        public static void SetValueToObject(object srcObj, IStorageObject desObj, string propertyName, Type propertyType)
        {
            bool nullable = IsNullable(propertyType);
            object obj2 = ToObject(srcObj, propertyType, nullable);
            desObj.SetValue(propertyName, obj2);
        }

        private static string ToHexString(byte[] data)
        {
            string str = "0x";
            int length = data.Length;
            for (int i = 0; i < length; i++)
            {
                str = str + data[i].ToString("X2");
            }
            return str;
        }

        public static object ToObject(object obj, Type type, bool nullable)
        {
            object obj2;
            if (Convert.IsDBNull(obj))
            {
                if (IsNullable(type))
                {
                    return null;
                }
                if (((((type.Equals(typeof(byte)) || type.Equals(typeof(sbyte))) || (type.Equals(typeof(short)) || type.Equals(typeof(ushort)))) || ((type.Equals(typeof(int)) || type.Equals(typeof(uint))) || (type.Equals(typeof(long)) || type.Equals(typeof(ulong))))) || ((type.Equals(typeof(char)) || type.Equals(typeof(float))) || type.Equals(typeof(double)))) || type.Equals(typeof(decimal)))
                {
                    return 0;
                }
                if (type.Equals(typeof(string)))
                {
                    return "";
                }
                if (type.Equals(typeof(DateTime)))
                {
                    return DateTime.Now;
                }
                return obj;
            }
            if (type.IsInstanceOfType(obj))
            {
                return obj;
            }
            if (!(!nullable || type.Equals(typeof(string))))
            {
                type = Nullable.GetUnderlyingType(type);
            }
            if (type.IsEnum)
            {
                obj2 = Enum.ToObject(type, obj);
            }
            else if (type.Equals(typeof(TimeSpan)))
            {
                obj2 = TimeSpan.Parse(obj as string);
            }
            else
            {
                obj2 = Convert.ChangeType(obj, type, null);
            }
            if (type.Equals(typeof(DateTime)))
            {
                DateTime time = (DateTime) obj2;
                obj2 = new DateTime(time.Ticks, DateTimeKind.Local);
            }
            return obj2;
        }

        public static string ToSQL(object value, bool allowDBNull = false)
        {
            if (value == null)
            {
                if (!allowDBNull)
                {
                    throw new ORMException("Convert Value Error In [" + value + "]");
                }
                return "null";
            }
            if (value.GetType() == typeof(string))
            {
                string str = (string) value;
                return Escape(str);
            }
            if (value.GetType() == typeof(Guid))
            {
                return value.ToString();
            }
            if (value.GetType() == typeof(DateTime))
            {
                DateTime time = (DateTime) value;
                if (time.Equals(DateTime.MinValue) || time.Equals(DateTime.MaxValue))
                {
                    return "null";
                }
                return GetDateString((DateTime) value);
            }
            if (value.GetType() == typeof(DateTimeOffset))
            {
                DateTimeOffset date = (DateTimeOffset) value;
                if (date.Equals(DateTimeOffset.MinValue) || date.Equals(DateTimeOffset.MaxValue))
                {
                    return "null";
                }
                return GetDateTimeOffsetString(date);
            }
            if (value.GetType() == typeof(TimeSpan))
            {
                return value.ToString();
            }
            if (value.GetType() == typeof(bool))
            {
                if ((bool) value)
                {
                    return "1";
                }
                return "0";
            }
            if (value.GetType().IsEnum)
            {
                return Convert.ToInt32(value).ToString();
            }
            if (value is byte[])
            {
                return ToHexString(value as byte[]);
            }
            return (value);
        }
    }
}

