namespace EgoalTech.DB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class DbObjectTools
    {
        internal static T Clone<T>(object obj) where T: new()
        {
            T des = new T();
            CloneUtils.CloneObject(obj, des, new string[0]);
            return des;
        }

        internal static bool Compare(IStorageObject obj1, IStorageObject obj2)
        {
            if (!obj1.GetType().Equals(obj2.GetType()))
            {
                return false;
            }
            DbObjectInfo dbObjectInfo = DbObjectReflector.GetDbObjectInfo(obj1.GetType());
            foreach (KeyValuePair<string, DynamicPropertyInfo> pair in dbObjectInfo.DynamicPropertyInfos)
            {
                object obj3 = obj1.GetValue(pair.Value.PropertyName);
                object obj4 = obj2.GetValue(pair.Value.PropertyName);
                if (obj3 != obj4)
                {
                    return false;
                }
            }
            return true;
        }

        public static DbObject CreateDbObject(Type objectType)
        {
            return (DbObject) Activator.CreateInstance(objectType);
        }

        public static DynamicPropertyInfo GetAutoIncrementDynamicPropertyInfo(DbObjectInfo dbObjectInfo)
        {
            return (from info in dbObjectInfo.DynamicPropertyInfos
                where info.Value.AutoIncrement
                select info.Value).FirstOrDefault<DynamicPropertyInfo>();
        }

        public static DataFieldAttribute GetDataFieldAttribute(FieldInfo field)
        {
            DataFieldAttribute[] customAttributes = (DataFieldAttribute[]) field.GetCustomAttributes(typeof(DataFieldAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                if (customAttributes[0].DataFieldName.Length == 0)
                {
                    customAttributes[0].DataFieldName = field.Name;
                }
                if (customAttributes[0].ObjectFieldName.Length == 0)
                {
                    customAttributes[0].ObjectFieldName = field.Name;
                }
                customAttributes[0].FieldType = field.FieldType;
                return customAttributes[0];
            }
            return null;
        }

        public static DataFieldAttribute GetDataFieldAttribute(PropertyInfo property)
        {
            DataFieldAttribute[] customAttributes = (DataFieldAttribute[]) property.GetCustomAttributes(typeof(DataFieldAttribute), true);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                if (customAttributes[0].DataFieldName.Length == 0)
                {
                    customAttributes[0].DataFieldName = property.Name;
                }
                if (customAttributes[0].ObjectFieldName.Length == 0)
                {
                    customAttributes[0].ObjectFieldName = property.Name;
                }
                customAttributes[0].FieldType = property.PropertyType;
                return customAttributes[0];
            }
            return null;
        }

        public static DataFieldAttribute[] GetDataFieldAttributes(object obj)
        {
            DataFieldAttribute dataFieldAttribute;
            int num;
            List<DataFieldAttribute> list = new List<DataFieldAttribute>();
            FieldInfo[] fields = GetFields(obj.GetType());
            int length = fields.Length;
            for (num = 0; num < length; num++)
            {
                dataFieldAttribute = GetDataFieldAttribute(fields[num]);
                if (dataFieldAttribute != null)
                {
                    list.Add(dataFieldAttribute);
                }
            }
            PropertyInfo[] properties = GetProperties(obj.GetType());
            length = properties.Length;
            for (num = 0; num < length; num++)
            {
                dataFieldAttribute = GetDataFieldAttribute(properties[num]);
                if (dataFieldAttribute != null)
                {
                    list.Add(dataFieldAttribute);
                }
            }
            return list.ToArray();
        }

        public static string GetDataFieldName<T>(PropertyInfo property) where T: DbObject
        {
            Type type = typeof(T);
            return GetDataFieldName(type, property);
        }

        public static string GetDataFieldName(Type type, PropertyInfo property)
        {
            DbObjectInfo dbObjectInfo = GetDbObjectInfo(type);
            if (dbObjectInfo.DynamicPropertyInfos != null)
            {
                DynamicPropertyInfo info2 = (from t in dbObjectInfo.DynamicPropertyInfos.Values
                    where t.PropertyName.Equals(property.Name)
                    select t).FirstOrDefault<DynamicPropertyInfo>();
                if (info2 != null)
                {
                    return info2.DataFieldName;
                }
            }
            return null;
        }

        public static DbObjectInfo GetDbObjectInfo(Type type)
        {
            DbObjectInfo dbObjectInfo = DbObjectReflector.GetDbObjectInfo(type);
            if (dbObjectInfo == null)
            {
                throw new Exception("Cannot retrieve DbObjectInfo of type : " + type.ToString());
            }
            return dbObjectInfo;
        }

        public static DynamicPropertyInfo GetDynamicPropertyInfo(Type type, PropertyInfo propertyInfo)
        {
            return (from info in GetDbObjectInfo(type).DynamicPropertyInfos
                where info.Value.PropertyName.Equals(propertyInfo.Name)
                select info.Value).FirstOrDefault<DynamicPropertyInfo>();
        }

        public static DynamicPropertyInfo GetDynamicPropertyInfo(Type type, string propertyName)
        {
            return (from info in GetDbObjectInfo(type).DynamicPropertyInfos
                where info.Value.PropertyName.Equals(propertyName)
                select info.Value).FirstOrDefault<DynamicPropertyInfo>();
        }

        public static FieldInfo GetField(Type type, string name)
        {
            FieldInfo field = null;
            field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if ((field == null) && IsIncludeBaseType(type))
            {
                field = GetField(type.BaseType, name);
            }
            return field;
        }

        public static FieldInfo[] GetFields(Type type)
        {
            List<FieldInfo> list = new List<FieldInfo>();
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (fields != null)
            {
                list.AddRange(fields);
            }
            if (IsIncludeBaseType(type))
            {
                fields = GetFields(type.BaseType);
                if (fields != null)
                {
                    list.AddRange(fields);
                }
            }
            return list.ToArray();
        }

        public static DynamicPropertyInfo GetPkDynamicPropertyInfo(DbObjectInfo dbObjectInfo, bool checkExist = true)
        {
            DynamicPropertyInfo info = (from inf in dbObjectInfo.DynamicPropertyInfos
                where inf.Value.PrimaryKey
                select inf.Value).SingleOrDefault<DynamicPropertyInfo>();
            if ((info == null) & checkExist)
            {
                throw new Exception("Cannot find primary key property of table (or view): " + dbObjectInfo.TableName);
            }
            return info;
        }

        public static PropertyInfo[] GetProperties(Type type)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (properties != null)
            {
                list.AddRange(properties);
            }
            if (IsIncludeBaseType(type))
            {
                properties = GetProperties(type.BaseType);
                if (properties != null)
                {
                    list.AddRange(properties);
                }
            }
            return list.ToArray();
        }

        public static PropertyInfo GetProperty(Type type, string name)
        {
            PropertyInfo property = null;
            property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if ((property == null) && IsIncludeBaseType(type))
            {
                property = GetProperty(type.BaseType, name);
            }
            return property;
        }

        public static string GetQueryName(Type type)
        {
            DbObjectInfo dbObjectInfo = GetDbObjectInfo(type);
            if (dbObjectInfo.QueryTable == null)
            {
                return null;
            }
            return dbObjectInfo.QueryTable;
        }

        public static string GetTableName(Type type)
        {
            DbObjectInfo dbObjectInfo = GetDbObjectInfo(type);
            if (dbObjectInfo.TableName == null)
            {
                return null;
            }
            return dbObjectInfo.TableName;
        }

        public static string GetViewOrTableName(Type type)
        {
            DbObjectInfo dbObjectInfo = GetDbObjectInfo(type);
            if (!string.IsNullOrEmpty(dbObjectInfo.QueryTable))
            {
                return dbObjectInfo.QueryTable;
            }
            return dbObjectInfo.TableName;
        }

        public static bool IsIncludeBaseType(Type type)
        {
            IncludeBaseTypeAttribute[] customAttributes = null;
            customAttributes = (IncludeBaseTypeAttribute[]) type.GetCustomAttributes(typeof(IncludeBaseTypeAttribute), true);
            return ((customAttributes != null) && (customAttributes.Length > 0));
        }
    }
}

