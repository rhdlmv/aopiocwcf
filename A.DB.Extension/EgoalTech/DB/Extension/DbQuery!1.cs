namespace EgoalTech.DB.Extension
{
    using EgoalTech.DB;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public class DbQuery<T> : DbQuery
    {
        private string methodFormat;
        private Expression<Func<T, object>> selectField;

        public DbQuery()
        {
        }

        public DbQuery(DbQuery<T> query) : this(query.Condition)
        {
        }

        public DbQuery(Expression<Func<T, bool>> condition)
        {
            if (condition != null)
            {
                this.Parameter = condition.Parameters.First<ParameterExpression>();
                this.Body = condition.Body;
            }
        }

        public DbQuery<T> Avg(Expression<Func<T, object>> field)
        {
            this.methodFormat = "Avg(1. * {0})";
            this.selectField = field;
            return (DbQuery<T>) this;
        }

        public DbQuery<T> Count()
        {
            this.methodFormat = "COUNT({0})";
            return (DbQuery<T>) this;
        }

        public DbQuery<T> Distinct()
        {
            this.methodFormat = "DISTINCT({0})";
            return (DbQuery<T>) this;
        }

        public DbQuery<T> Max(Expression<Func<T, object>> field)
        {
            this.methodFormat = "MAX({0})";
            this.selectField = field;
            return (DbQuery<T>) this;
        }

        public DbQuery<T> Min(Expression<Func<T, object>> field)
        {
            this.methodFormat = "MIN({0})";
            this.selectField = field;
            return (DbQuery<T>) this;
        }

        public static DbQuery<T> operator &(DbQuery<T> condition, DbQuery<T> joinCondition)
        {
            if (joinCondition == null)
            {
                return condition;
            }
            return (condition & joinCondition.Condition);
        }

        public static DbQuery<T> operator &(DbQuery<T> condition, Expression<Func<T, bool>> joinCondition)
        {
            if ((joinCondition != null) && (joinCondition.Body != null))
            {
                condition = condition ?? new DbQuery<T>();
                if (condition.Body != null)
                {
                    condition.Body = Expression.AndAlso(condition.Body, joinCondition.Body);
                }
                else
                {
                    condition.Body = joinCondition.Body;
                }
                condition.Parameter = condition.Parameter ?? joinCondition.Parameters.First<ParameterExpression>();
            }
            return condition;
        }

        public static DbQuery<T> operator |(DbQuery<T> condition, DbQuery<T> joinCondition)
        {
            if (joinCondition == null)
            {
                return condition;
            }
            return (condition | joinCondition.Condition);
        }

        public static DbQuery<T> operator |(DbQuery<T> condition, Expression<Func<T, bool>> joinCondition)
        {
            if ((joinCondition != null) && (joinCondition.Body != null))
            {
                condition = condition ?? new DbQuery<T>();
                if (condition.Body != null)
                {
                    condition.Body = Expression.OrElse(condition.Body, joinCondition.Body);
                }
                else
                {
                    condition.Body = joinCondition.Body;
                }
                condition.Parameter = condition.Parameter ?? joinCondition.Parameters.First<ParameterExpression>();
            }
            return condition;
        }

        public static implicit operator DbQuery<T>(Expression<Func<T, bool>> condition) => 
            new DbQuery<T>(condition)

        public DbQuery<T> Select(Expression<Func<T, object>> field)
        {
            this.selectField = field;
            return (DbQuery<T>) this;
        }

        public DbQuery<T> Sum(Expression<Func<T, object>> field)
        {
            this.methodFormat = "Sum({0})";
            this.selectField = field;
            return (DbQuery<T>) this;
        }

        public override string ToSql(ExpressionConverter expressionRouter, params object[] skipParameters)
        {
            string condition = expressionRouter.ToCondition(this.Condition, skipParameters);
            if (this.selectField == null)
            {
                return condition;
            }
            string fieldName = expressionRouter.ToFieldName(this.selectField);
            string viewOrTableName = DbObjectTools.GetViewOrTableName(typeof(T));
            viewOrTableName = expressionRouter.SqlFormatter.FormatTableOrViewName(viewOrTableName);
            fieldName = expressionRouter.SqlFormatter.FormatFieldName(fieldName);
            if (this.methodFormat != null)
            {
                fieldName = string.Format(this.methodFormat, fieldName);
            }
            return expressionRouter.SqlFormatter.FormatSelectSql(viewOrTableName, fieldName, condition);
        }

        public DbQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                this.Parameter = predicate.Parameters.First<ParameterExpression>();
                this.Body = predicate.Body;
            }
            return (DbQuery<T>) this;
        }

        private Expression Body { get; set; }

        public Expression<Func<T, bool>> Condition
        {
            get
            {
                if (this.Body == null)
                {
                    return null;
                }
                return Expression.Lambda<Func<T, bool>>(this.Body, new ParameterExpression[] { this.Parameter });
            }
        }

        private ParameterExpression Parameter { get; set; }
    }
}

