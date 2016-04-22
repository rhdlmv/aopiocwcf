using A.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using System.Text;

namespace A.DBExtension
{
    public class ExpressionConverter
    {
        public ExpressionConverter(ISqlFormatter sqlFormater)
        {
            this.SqlFormatter = sqlFormater;
        }

        private string BinaryToSql(BinaryExpression b, ParameterExpression[] parameters, object[] parameterValues, ParameterExpression parameter)
        {
            string str = this.ToConditionSql(b.Left, parameters, parameterValues, parameter);
            string str2 = this.ToConditionSql(b.Right, parameters, parameterValues, parameter);
            string operatorString = this.SqlFormatter.GetOperatorString(b.NodeType);
            if ((b.NodeType == ExpressionType.AndAlso) || (b.NodeType == ExpressionType.OrElse))
            {
                if (b.Left.NodeType == ExpressionType.Constant)
                {
                    this.ThrowErrorExpression(b.Left);
                }
                else if (b.Right.NodeType == ExpressionType.Constant)
                {
                    this.ThrowErrorExpression(b.Right);
                }
            }
            if (string.IsNullOrEmpty(operatorString))
            {
                this.ThrowErrorExpression(b);
            }
            if (str2 == "null")
            {
                if (b.NodeType == ExpressionType.Equal)
                {
                    operatorString = "is";
                }
                else if (b.NodeType == ExpressionType.NotEqual)
                {
                    operatorString = "is not";
                }
            }
            if (b.NodeType == ExpressionType.OrElse)
            {
                return ("((" + str + ") " + operatorString + " (" + str2 + "))");
            }
            return string.Format("{0} {1} {2}", str, operatorString, str2);
        }

        private string FormatMemberAccess(Expression exp)
        {
            MemberExpression expression = exp as MemberExpression;
            if (expression != null)
            {
                if (expression.Expression is ConstantExpression)
                {
                    return expression.Member.Name;
                }
                if (expression.Expression is MemberExpression)
                {
                    return (this.FormatMemberAccess(expression.Expression) + "." + expression.Member.Name);
                }
            }
            return null;
        }

        private string FormatValue(Expression exp, ParameterExpression[] parameters, object[] parameterValues)
        {
            string str = null;
            try
            {
                object obj2 = null;
                if (parameterValues.Length > 0)
                {
                    obj2 = Expression.Lambda(exp, parameters).Eval(parameterValues);
                }
                else
                {
                    obj2 = exp.Eval();
                }
                if (obj2 == null)
                {
                    return "null";
                }
                if (((obj2 is Array) || (obj2 is IList)) || (obj2 is IEnumerable<string>))
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (object obj3 in obj2 as IEnumerable)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(", ");
                        }
                        builder.Append(this.SqlFormatter.FormatValue(obj3, false));
                    }
                    str = builder.ToString();
                }
                else
                {
                    str = this.SqlFormatter.FormatValue(obj2, false);
                }
            }
            catch
            {
                this.ThrowErrorExpression(exp);
            }
            return str;
        }

        private string MemberAccessToSql(MemberExpression m, ParameterExpression[] parameters, object[] parameterValues, ParameterExpression parameter)
        {
            if (((m.Member.MemberType == MemberTypes.Property) && (m.Expression != null)) && (m.Expression.NodeType == ExpressionType.Parameter))
            {
                ParameterExpression expression = m.Expression as ParameterExpression;
                if (((parameters.Length == 1) || (expression == parameter)) || (expression.Name == parameter.Name))
                {
                    ParameterExpression expression2 = m.Expression as ParameterExpression;
                    PropertyInfo member = m.Member as PropertyInfo;
                    if (member != null)
                    {
                        DynamicPropertyInfo dynamicPropertyInfo = DbObjectTools.GetDynamicPropertyInfo(expression2.Type, member);
                        if (dynamicPropertyInfo != null)
                        {
                            return this.SqlFormatter.FormatFieldName(dynamicPropertyInfo.DataFieldName);
                        }
                    }
                }
            }
            return this.FormatValue(m, parameters, parameterValues);
        }

        private string MethodArgumentToSql(Expression exp, ParameterExpression[] parameters, object[] parameterValues, ParameterExpression parameter)
        {
            string str = this.ToConditionSql(exp, parameters, parameterValues, parameter);
            if (str == null)
            {
                str = this.FormatValue(exp, parameters, parameterValues);
            }
            return str;
        }

        private string MethodToSql(MethodCallExpression m, ParameterExpression[] parameters, object[] parameterValues, ParameterExpression parameter)
        {
            MethodInfo method = m.Method;
            if (method.IsGenericMethod)
            {
                method = method.GetGenericMethodDefinition();
            }
            string sqlMethodFormat = this.SqlFormatter.GetSqlMethodFormat(method);
            if (sqlMethodFormat == null)
            {
                return this.FormatValue(m, parameters, parameterValues);
            }
            List<object> list = new List<object>();
            foreach (Expression expression in m.Arguments)
            {
                string item = this.MethodArgumentToSql(expression, parameters, parameterValues, parameter);
                if (item == null)
                {
                    this.ThrowErrorExpression(m);
                }
                list.Add(item);
            }
            return string.Format(sqlMethodFormat, list.ToArray());
        }

        private void ThrowErrorExpression(Expression exp)
        {
            object obj2 = this.FormatMemberAccess(exp);
            throw new Exception(string.Format("The part of lambda expression can't be parsed to SQL: {0}.", obj2 ?? exp));
        }

        public string ToCondition(LambdaExpression exp, params object[] skipParameters)
        {
            ParameterExpression parameter = null;
            ParameterExpression[] parameters = null;
            object[] parameterValues = null;
            if (exp == null)
            {
                return null;
            }
            parameterValues = skipParameters.ToArray<object>();
            parameters = exp.Parameters.ToArray<ParameterExpression>();
            parameter = exp.Parameters.Skip<ParameterExpression>(skipParameters.Length).FirstOrDefault<ParameterExpression>();
            return this.ToConditionSql(exp, parameters, parameterValues, parameter);
        }

        private string ToConditionSql(Expression exp, ParameterExpression[] parameters, object[] parameterValues, ParameterExpression parameter)
        {
            if (exp.Type.GetInterfaces().Contains<Type>(typeof(IDbQuery)))
            {
                IDbQuery query = null;
                if (parameterValues.Length > 0)
                {
                    query = Expression.Lambda(exp, parameters).Eval(parameterValues) as IDbQuery;
                }
                else
                {
                    query = exp.Eval() as IDbQuery;
                }
                return query.ToSql(this, new object[0]);
            }
            switch (exp.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return this.BinaryToSql(exp as BinaryExpression, parameters, parameterValues, parameter);

                case ExpressionType.Call:
                    return this.MethodToSql(exp as MethodCallExpression, parameters, parameterValues, parameter);

                case ExpressionType.Constant:
                    {
                        object obj2 = ((ConstantExpression)exp).Value;
                        if (obj2 != null)
                        {
                            return this.SqlFormatter.FormatValue(obj2, false);
                        }
                        return "null";
                    }
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.TypeAs:
                    return this.ToConditionSql(((UnaryExpression)exp).Operand, parameters, parameterValues, parameter);

                case ExpressionType.Lambda:
                    return this.ToConditionSql(((LambdaExpression)exp).Body, parameters, parameterValues, parameter);

                case ExpressionType.MemberAccess:
                    return this.MemberAccessToSql((MemberExpression)exp, parameters, parameterValues, parameter);
            }
            return this.FormatValue(exp, parameters, parameterValues);
        }

        public string ToFieldName(LambdaExpression field)
        {
            string dataFieldName = null;
            if (field != null)
            {
                MemberExpression body = field.Body as MemberExpression;
                UnaryExpression expression2 = field.Body as UnaryExpression;
                if ((body == null) && (expression2 != null))
                {
                    body = expression2.Operand as MemberExpression;
                }
                if (body != null)
                {
                    Type[] genericArguments = field.Type.GetGenericArguments();
                    if ((body.Member.MemberType == MemberTypes.Property) && (body.Expression.NodeType == ExpressionType.Parameter))
                    {
                        PropertyInfo member = body.Member as PropertyInfo;
                        if (member != null)
                        {
                            dataFieldName = DbObjectTools.GetDataFieldName(genericArguments[0], member);
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(dataFieldName))
            {
                throw new Exception("無法從 Lambda 表達式 {0} 找到其對應的字段名稱。");
            }
            return dataFieldName;
        }

        public ISqlFormatter SqlFormatter { get; private set; }
    }
}

