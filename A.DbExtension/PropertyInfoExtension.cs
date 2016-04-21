namespace A.DBExtension
{
    using EgoalTech.DB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class PropertyInfoExtension
    {
        private static Dictionary<PropertyInfo, DataFieldAttribute> dataFieldAttributes = new Dictionary<PropertyInfo, DataFieldAttribute>();
        private static Dictionary<MethodInfo, SqlMethodAttribute> sqlMethodAttributes = new Dictionary<MethodInfo, SqlMethodAttribute>();

        public static DataFieldAttribute GetDataFieldAttribute(this PropertyInfo property)
        {
            DataFieldAttribute attribute = null;
            if (!dataFieldAttributes.TryGetValue(property, out attribute))
            {
                lock (dataFieldAttributes)
                {
                    if (!dataFieldAttributes.TryGetValue(property, out attribute))
                    {
                        attribute = property.GetCustomAttributes(typeof(DataFieldAttribute), true).OfType<DataFieldAttribute>().FirstOrDefault<DataFieldAttribute>();
                        dataFieldAttributes[property] = attribute;
                    }
                }
            }
            return attribute;
        }

        public static SqlMethodAttribute GetSqlMethodAttribute(this MethodInfo method)
        {
            SqlMethodAttribute attribute = null;
            if (!sqlMethodAttributes.TryGetValue(method, out attribute))
            {
                lock (sqlMethodAttributes)
                {
                    if (!sqlMethodAttributes.TryGetValue(method, out attribute))
                    {
                        attribute = method.GetCustomAttributes(typeof(SqlMethodAttribute), true).OfType<SqlMethodAttribute>().FirstOrDefault<SqlMethodAttribute>();
                        sqlMethodAttributes[method] = attribute;
                    }
                }
            }
            return attribute;
        }
    }
}

