namespace EgoalTech.DB.Extension
{
    using EgoalTech.DB;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

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

        private static Dictionary<ExpressionType, string> InitOperatorStrings() => 
            new Dictionary<ExpressionType, string> { 
                { 
                    ExpressionType.Add,
                    "+"
                },
                { 
                    ExpressionType.AddChecked,
                    "+"
                },
                { 
                    ExpressionType.And,
                    "&"
                },
                { 
                    ExpressionType.AndAlso,
                    "AND"
                },
                { 
                    ExpressionType.Divide,
                    "/"
                },
                { 
                    ExpressionType.Decrement,
                    "-1"
                },
                { 
                    ExpressionType.Equal,
                    "="
                },
                { 
                    ExpressionType.ExclusiveOr,
                    "^"
                },
                { 
                    ExpressionType.GreaterThan,
                    ">"
                },
                { 
                    ExpressionType.GreaterThanOrEqual,
                    ">="
                },
                { 
                    ExpressionType.Increment,
                    "+1"
                },
                { 
                    ExpressionType.LessThan,
                    "<"
                },
                { 
                    ExpressionType.LessThanOrEqual,
                    "<="
                },
                { 
                    ExpressionType.Modulo,
                    "%"
                },
                { 
                    ExpressionType.Multiply,
                    "*"
                },
                { 
                    ExpressionType.MultiplyChecked,
                    "*"
                },
                { 
                    ExpressionType.NotEqual,
                    "<>"
                },
                { 
                    ExpressionType.Or,
                    "|"
                },
                { 
                    ExpressionType.OrElse,
                    "OR"
                },
                { 
                    ExpressionType.Subtract,
                    "-"
                },
                { 
                    ExpressionType.SubtractChecked,
                    "-"
                }
            }
    }
}

