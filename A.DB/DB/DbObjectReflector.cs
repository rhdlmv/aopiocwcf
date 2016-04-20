namespace EgoalTech.DB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class DbObjectReflector
    {
        private static Dictionary<Type, DbObjectInfo> structures = new Dictionary<Type, DbObjectInfo>();

        public static DbObjectInfo GetDbObjectInfo(Type type)
        {
            if (structures.ContainsKey(type))
            {
                return structures[type];
            }
            object[] customAttributes = type.GetCustomAttributes(true);
            if (customAttributes != null)
            {
                DataTableAttribute attribute = (from a in customAttributes
                    where a is DataTableAttribute
                    select a).Cast<DataTableAttribute>().FirstOrDefault<DataTableAttribute>();
                QueryTableAttribute attribute2 = (from a in customAttributes
                    where a is QueryTableAttribute
                    select a).Cast<QueryTableAttribute>().FirstOrDefault<QueryTableAttribute>();
                if ((attribute == null) && (attribute2 == null))
                {
                    return null;
                }
                DbObjectInfo info = new DbObjectInfo();
                if (attribute != null)
                {
                    info.TableName = attribute.TableName;
                    info.DatabaseName = attribute.DatabaseName;
                }
                if (attribute2 != null)
                {
                    info.QueryTable = attribute2.TableName;
                }
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo info2 in properties)
                {
                    DataFieldAttribute dataFieldAttribute = DbObjectTools.GetDataFieldAttribute(info2);
                    if (dataFieldAttribute != null)
                    {
                        DynamicPropertyInfo des = new DynamicPropertyInfo();
                        CloneUtils.CloneObject(dataFieldAttribute, des, new string[0]);
                        des.PropertyName = info2.Name;
                        des.PropertyType = info2.PropertyType;
                        info.AddDynamicPropertyInfo(des.PropertyName, des);
                    }
                }
                Register(type, info);
                return info;
            }
            return null;
        }

        public static void Register(Type type, DbObjectInfo info)
        {
            if (structures.ContainsKey(type))
            {
                structures[type] = info;
            }
            else
            {
                structures.Add(type, info);
            }
        }
    }
}

