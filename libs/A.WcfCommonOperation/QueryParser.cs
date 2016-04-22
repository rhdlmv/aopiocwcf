namespace A.WcfCommonOperation
{
    using A.DB;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;

    public class QueryParser
    {
        private static string[] connector = new string[] { "AND", "OR" };
        private static char leftBracket = '(';
        private static Regex nonOperator = new Regex("^[0-9A-Za-z '()]");
        private static string[] operators = new string[] { "=", "!=", "<>", ">", ">=", "<=", "<", "LIKE", "IS", "NOT", "IN" };
        private static string[] order = new string[] { "ASC", "DESC" };
        private static char quote = '\'';
        private static char rightBracket = ')';

        public string Escape(string str)
        {
            return str.Replace("'", "''");
        }

        private string PadSpace(string str)
        {
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            for (int i = 0; i < str.Length; i++)
            {
                bool flag2 = false;
                char ch = str[i];
                if (ch == '\'')
                {
                    flag = !flag;
                }
                else if ((!flag && (i > 0)) && ((i + 1) < str.Length))
                {
                    char ch2 = str[i - 1];
                    char ch3 = str[i + 1];
                    if ((!nonOperator.IsMatch(Convert.ToString(ch)) && !nonOperator.IsMatch(Convert.ToString(ch3))) || ((nonOperator.IsMatch(Convert.ToString(ch)) && nonOperator.IsMatch(Convert.ToString(ch2))) && nonOperator.IsMatch(Convert.ToString(ch3))))
                    {
                        flag2 = false;
                    }
                    else
                    {
                        flag2 = true;
                    }
                }
                builder.Append(str[i]);
                if (flag2)
                {
                    builder.Append(' ');
                }
            }
            return builder.ToString();
        }

        public string Parse(string queryStr, Type type)
        {
            string str;
            int num;
            if (string.IsNullOrWhiteSpace(queryStr))
            {
                return "";
            }
            queryStr = queryStr.Trim();
            queryStr = this.PadSpace(queryStr);
            StringBuilder builder = new StringBuilder();
            List<string> list = new List<string>();
            StringBuilder builder2 = new StringBuilder();
            for (num = 0; num < queryStr.Length; num++)
            {
                if ((queryStr[num] == leftBracket) || (queryStr[num] == rightBracket))
                {
                    str = builder2.ToString().Trim();
                    builder2.Clear();
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        list.Add(str);
                    }
                    list.Add(queryStr[num].ToString());
                }
                else
                {
                    builder2.Append(queryStr[num]);
                }
            }
            str = builder2.ToString().Trim();
            if (!string.IsNullOrWhiteSpace(str))
            {
                list.Add(str);
            }
            bool flag = false;
            for (num = 0; num < list.Count; num++)
            {
                string last = (num > 0) ? list[num - 1].Trim() : null;
                string next = ((num + 1) < list.Count) ? list[num + 1].Trim() : null;
                if (list[num].Equals(leftBracket.ToString()))
                {
                    if ((last != null) && last.ToUpper().EndsWith(" IN"))
                    {
                        flag = true;
                    }
                    builder.Append(list[num]);
                }
                else if (list[num].Equals(rightBracket.ToString()))
                {
                    if (flag)
                    {
                        flag = false;
                    }
                    builder.Append(list[num]);
                }
                else if (flag)
                {
                    builder.Append(this.ParseInFragment(list[num]));
                }
                else
                {
                    builder.Append(this.ParseFragment(list[num], last, next, type));
                }
            }
            return builder.ToString();
        }

        private string ParseFragment(string fragment, string last, string next, Type type)
        {
            if (string.IsNullOrWhiteSpace(fragment))
            {
                return "";
            }
            string[] splits = Split(fragment, ' ', new char?(quote));
            this.ValidateFragment(splits, last, next);
            StringBuilder builder = new StringBuilder();
            int num = 0;
            if (connector.Contains<string>(splits[0].ToUpper()))
            {
                builder.Append(" ");
                builder.Append(splits[0]);
                builder.Append(" ");
                num++;
            }
            string propertyStr = "";
            for (int i = num; i < splits.Length; i++)
            {
                if ((((i + 4) - num) % 4) == 0)
                {
                    if (operators.Contains<string>(splits[i].ToUpper()) || connector.Contains<string>(splits[i].ToUpper()))
                    {
                        throw new QuerySyntaxException();
                    }
                    string str2 = this.ProcessProperty(splits[i], type);
                    if (str2 == null)
                    {
                        throw new QuerySyntaxException(string.Format("The property '{0}' is not exists", splits[i]));
                    }
                    builder.Append(str2);
                    propertyStr = splits[i];
                }
                else if ((((i + 4) - num) % 4) == 1)
                {
                    if (!operators.Contains<string>(splits[i].ToUpper()))
                    {
                        throw new QuerySyntaxException();
                    }
                    builder.Append(" ");
                    if ("NOT".Equals(splits[i].ToUpper()))
                    {
                        builder.Append("IS NOT");
                    }
                    else
                    {
                        builder.Append(splits[i]);
                    }
                    builder.Append(" ");
                }
                else if ((((i + 4) - num) % 4) == 2)
                {
                    if (operators.Contains<string>(splits[i].ToUpper()) || connector.Contains<string>(splits[i].ToUpper()))
                    {
                        throw new QuerySyntaxException();
                    }
                    if ("NOT".Equals(splits[i - 1].ToUpper()) || "IS".Equals(splits[i - 1].ToUpper()))
                    {
                        if (!"NULL".Equals(splits[i].ToUpper()))
                        {
                            throw new QuerySyntaxException();
                        }
                        builder.Append("NULL");
                    }
                    else
                    {
                        builder.Append(this.ProcessValue(splits[i], propertyStr, type));
                    }
                    builder.Append(" ");
                }
                else
                {
                    if (!connector.Contains<string>(splits[i].ToUpper()))
                    {
                        throw new QuerySyntaxException();
                    }
                    builder.Append(splits[i]);
                    builder.Append(" ");
                }
            }
            return builder.ToString();
        }

        private string ParseInFragment(string fragment)
        {
            if (string.IsNullOrEmpty(fragment))
            {
                throw new QuerySyntaxException("Incorrent value syntax of IN statement.");
            }
            StringBuilder builder = new StringBuilder();
            string[] strArray = fragment.Split(new char[] { ',' });
            for (int i = 0; i < strArray.Length; i++)
            {
                string str = strArray[i].Trim();
                int num2 = 0;
                for (int j = 0; j < str.Length; j++)
                {
                    if (j == 0)
                    {
                        if ('\'' != str[j])
                        {
                            throw new QuerySyntaxException("Incorrent value syntax of IN statement.");
                        }
                    }
                    else if ((j + 1) == str.Length)
                    {
                        if ('\'' != str[j])
                        {
                            throw new QuerySyntaxException("Incorrent value syntax of IN statement.");
                        }
                    }
                    else if ('\'' == str[j])
                    {
                        if (('\'' != str[j - 1]) && ('\'' != str[j + 1]))
                        {
                            throw new QuerySyntaxException("Incorrent value syntax of IN statement.");
                        }
                        num2++;
                    }
                    else
                    {
                        if ((num2 % 2) != 0)
                        {
                            throw new QuerySyntaxException("Incorrent value syntax of IN statement.");
                        }
                        num2 = 0;
                    }
                    builder.Append(str[j]);
                }
                if ((i + 1) < strArray.Length)
                {
                    builder.Append(", ");
                }
            }
            return builder.ToString();
        }

        public OrderBy[] ParseSort(string sortStr, Type type)
        {
            if (string.IsNullOrWhiteSpace(sortStr))
            {
                return null;
            }
            List<OrderBy> list = new List<OrderBy>();
            string[] strArray = sortStr.Split(new char[] { ',' });
            foreach (string str in strArray)
            {
                char? ignoreSeparator = null;
                string[] strArray2 = Split(str.Trim(), ' ', ignoreSeparator);
                if (strArray2.Length != 2)
                {
                    throw new SortSyntaxException("Syntax Error: " + sortStr);
                }
                string str2 = this.ProcessProperty(strArray2[0], type);
                if (str2 == null)
                {
                    throw new SortSyntaxException("Syntax Error: " + sortStr);
                }
                if (!order.Contains<string>(strArray2[1].ToUpper()))
                {
                    throw new SortSyntaxException("Syntax Error: " + sortStr);
                }
                OrderBy item = new OrderBy {
                    Column = str2,
                    Desc = "DESC".Equals(strArray2[1].ToUpper())
                };
                list.Add(item);
            }
            return list.ToArray();
        }

        private string ProcessProperty(string propertyStr, Type type)
        {
            PropertyInfo property = DbObjectTools.GetProperty(type, propertyStr);
            if (property == null)
            {
                return null;
            }
            DataFieldAttribute dataFieldAttribute = DbObjectTools.GetDataFieldAttribute(property);
            if (dataFieldAttribute == null)
            {
                return null;
            }
            return dataFieldAttribute.DataFieldName;
        }

        private string ProcessValue(string valueStr, string propertyStr, Type type)
        {
            PropertyInfo property = DbObjectTools.GetProperty(type, propertyStr);
            if (((((property.PropertyType.Equals(typeof(byte)) || property.PropertyType.Equals(typeof(sbyte))) || (property.PropertyType.Equals(typeof(short)) || property.PropertyType.Equals(typeof(ushort)))) || ((property.PropertyType.Equals(typeof(int)) || property.PropertyType.Equals(typeof(uint))) || (property.PropertyType.Equals(typeof(long)) || property.PropertyType.Equals(typeof(ulong))))) || ((property.PropertyType.Equals(typeof(char)) || property.PropertyType.Equals(typeof(float))) || property.PropertyType.Equals(typeof(double)))) || property.PropertyType.Equals(typeof(decimal)))
            {
                return valueStr;
            }
            if (property.PropertyType.Equals(typeof(bool)))
            {
                if ("TRUE".Equals(valueStr.ToUpper()))
                {
                    return "1";
                }
                if ("1".Equals(valueStr))
                {
                    return "1";
                }
                return "0";
            }
            if ("''".Equals(valueStr))
            {
                valueStr = "";
            }
            else
            {
                valueStr = valueStr.Replace("''", "'");
            }
            return ("'" + this.Escape(valueStr) + "'");
        }

        private static string[] Split(string str, char separator, char? ignoreSeparator = new char?())
        {
            string str2;
            if (str == null)
            {
                return null;
            }
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            int num = 0;
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                if (ch == separator)
                {
                    if (!flag)
                    {
                        str2 = builder.ToString();
                        if ((str2 != null) && (str2.Length > 0))
                        {
                            list.Add(str2);
                        }
                        builder.Clear();
                    }
                    else
                    {
                        builder.Append(ch);
                    }
                }
                else
                {
                    char? nullable3 = ignoreSeparator;
                    int? nullable4 = nullable3.HasValue ? new int?(nullable3.GetValueOrDefault()) : null;
                    if (nullable4.HasValue && ignoreSeparator.Value.Equals(ch))
                    {
                        char? nullable = null;
                        char? nullable2 = null;
                        if ((i + 1) < str.Length)
                        {
                            nullable = new char?(str[i + 1]);
                        }
                        if (i > 0)
                        {
                            nullable2 = new char?(str[i - 1]);
                        }
                        if (flag)
                        {
                            num++;
                            nullable3 = nullable;
                            nullable4 = nullable3.HasValue ? new int?(nullable3.GetValueOrDefault()) : null;
                            if ((nullable4.HasValue && ignoreSeparator.Value.Equals(nullable)) && (num == 1))
                            {
                                builder.Append(ch);
                            }
                            else
                            {
                                nullable3 = nullable2;
                                nullable4 = nullable3.HasValue ? new int?(nullable3.GetValueOrDefault()) : null;
                                if ((nullable4.HasValue && ignoreSeparator.Value.Equals(nullable2)) && (num == 2))
                                {
                                    builder.Append(ch);
                                    num = 0;
                                }
                                else
                                {
                                    num = 0;
                                    flag = !flag;
                                }
                            }
                        }
                        else
                        {
                            if (flag)
                            {
                                num = 0;
                            }
                            flag = !flag;
                        }
                    }
                    else
                    {
                        builder.Append(ch);
                    }
                }
            }
            str2 = builder.ToString();
            if ((str2 != null) && (str2.Length > 0))
            {
                list.Add(str2);
            }
            return list.ToArray();
        }

        private void ValidateFragment(string[] splits, string last, string next)
        {
            if (leftBracket.ToString().Equals(next))
            {
                if (rightBracket.ToString().Equals(last))
                {
                    if (!((connector.Contains<string>(splits[0].ToUpper()) || connector.Contains<string>(splits[splits.Length - 1].ToUpper())) || "IN".Equals(splits[splits.Length - 1].ToUpper())))
                    {
                        throw new QuerySyntaxException();
                    }
                }
                else if (connector.Contains<string>(splits[0].ToUpper()) || (!connector.Contains<string>(splits[splits.Length - 1].ToUpper()) && !"IN".Equals(splits[splits.Length - 1].ToUpper())))
                {
                    throw new QuerySyntaxException();
                }
            }
            if (rightBracket.ToString().Equals(next))
            {
                if (leftBracket.ToString().Equals(last))
                {
                    if (connector.Contains<string>(splits[0].ToUpper()) || connector.Contains<string>(splits[splits.Length - 1].ToUpper()))
                    {
                        throw new QuerySyntaxException();
                    }
                }
                else if (!connector.Contains<string>(splits[0].ToUpper()))
                {
                    throw new QuerySyntaxException();
                }
            }
        }
    }
}

