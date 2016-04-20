namespace EgoalTech.DB.Extension
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class ExpressionExtension
    {
        public static object Eval(this Expression expr) => 
            expr.Eval<object>()

        public static T Eval<T>(this Expression expr)
        {
            if (expr == null)
            {
                return default(T);
            }
            ConstantExpression expression = expr as ConstantExpression;
            if (expression != null)
            {
                return (T) expression.Value;
            }
            MemberExpression expression2 = expr as MemberExpression;
            if (expression2 != null)
            {
                FieldInfo member = expression2.Member as FieldInfo;
                PropertyInfo info2 = expression2.Member as PropertyInfo;
                if (member != null)
                {
                    return (T) member.GetValue(expression2.Expression.Eval<object>());
                }
                if (info2 != null)
                {
                    return (T) info2.GetValue(expression2.Expression.Eval<object>(), null);
                }
            }
            Expression<Func<object>> expression3 = Expression.Lambda<Func<object>>(Expression.Convert(expr, typeof(object)), new ParameterExpression[0]);
            return expression3.Compile()();
        }

        public static object Eval(this LambdaExpression expr, object[] parameters) => 
            expr.Eval<object>(parameters)

        public static T Eval<T>(this LambdaExpression expr, object[] parameters)
        {
            if (expr == null)
            {
                return default(T);
            }
            ConstantExpression body = expr.Body as ConstantExpression;
            if (body != null)
            {
                return (T) body.Value;
            }
            ParameterExpression expression2 = expr.Body as ParameterExpression;
            if (expression2 != null)
            {
                int index = expr.Parameters.IndexOf(expression2);
                if (index < parameters.Length)
                {
                    return (T) parameters[index];
                }
            }
            MemberExpression expression3 = expr.Body as MemberExpression;
            if (expression3 != null)
            {
                object obj2;
                FieldInfo member = expression3.Member as FieldInfo;
                PropertyInfo info2 = expression3.Member as PropertyInfo;
                if (member != null)
                {
                    obj2 = Expression.Lambda(expression3.Expression, expr.Parameters).Eval<object>(parameters);
                    return (T) member.GetValue(obj2);
                }
                if (info2 != null)
                {
                    obj2 = Expression.Lambda(expression3.Expression, expr.Parameters).Eval<object>(parameters);
                    return (T) info2.GetValue(obj2, null);
                }
            }
            return (T) expr.Compile().DynamicInvoke(parameters);
        }
    }
}

