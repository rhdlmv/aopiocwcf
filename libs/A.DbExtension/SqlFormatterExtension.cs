using A.DB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;


namespace A.DBExtension
{
    public static class SqlFormatterExtension
    {
        private static Dictionary<ExpressionType, string> dicOperatorStrings = InitOperatorStrings();
        private static Dictionary<MethodInfo, string> dicSqlMethodFormats = new Dictionary<MethodInfo, string>();

        public static string FormatSelectSql(this ISqlFormatter formatter, string tableOrView, string fields, string condition)
        {
            string selectSqlTemplate = formatter.GetSelectSqlTemplate();
            string str2 = "";
            return string.Format(selectSqlTemplate, new object[] { fields, tableOrView, str2, condition });
        }

        public static string GetOperatorString(this ISqlFormatter formatter, ExpressionType type)
        {
            string str = null;
            dicOperatorStrings.TryGetValue(type, out str);
            return str;
        }

        public static string GetSqlMethodFormat(this ISqlFormatter formatter, MethodInfo method)
        {
            string str = null;
            if (dicSqlMethodFormats.TryGetValue(method, out str) || !method.IsDefined(typeof(SqlMethodAttribute), true))
            {
                return str;
            }
            SqlMethodAttribute sqlMethodAttribute = method.GetSqlMethodAttribute();
            lock (dicSqlMethodFormats)
            {
                if (!dicSqlMethodFormats.TryGetValue(method, out str))
                {
                    dicSqlMethodFormats.Add(method, sqlMethodAttribute.Format);
                }
            }
            return sqlMethodAttribute.Format;
        }

        private static Dictionary<ExpressionType, string> InitOperatorStrings()
        {
            Dictionary<ExpressionType, string> dictionary = new Dictionary<ExpressionType, string>();
            dictionary.Add(ExpressionType.Add, "+");
            dictionary.Add(ExpressionType.AddChecked, "+");
            dictionary.Add(ExpressionType.And, "&");
            dictionary.Add(ExpressionType.AndAlso, "AND");
            dictionary.Add(ExpressionType.Divide, "/");
            dictionary.Add(ExpressionType.Decrement, "-1");
            dictionary.Add(ExpressionType.Equal, "=");
            dictionary.Add(ExpressionType.ExclusiveOr, "^");
            dictionary.Add(ExpressionType.GreaterThan, ">");
            dictionary.Add(ExpressionType.GreaterThanOrEqual, ">=");
            dictionary.Add(ExpressionType.Increment, "+1");
            dictionary.Add(ExpressionType.LessThan, "<");
            dictionary.Add(ExpressionType.LessThanOrEqual, "<=");
            dictionary.Add(ExpressionType.Modulo, "%");
            dictionary.Add(ExpressionType.Multiply, "*");
            dictionary.Add(ExpressionType.MultiplyChecked, "*");
            dictionary.Add(ExpressionType.NotEqual, "<>");
            dictionary.Add(ExpressionType.Or, "|");
            dictionary.Add(ExpressionType.OrElse, "OR");
            dictionary.Add(ExpressionType.Subtract, "-");
            dictionary.Add(ExpressionType.SubtractChecked, "-");
            return dictionary;
        }
    }
}

