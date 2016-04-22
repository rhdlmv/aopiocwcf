namespace A.DBExtension
{
    using System;

    public interface IDbQuery
    {
        string ToSql(ExpressionConverter expressionRouter, params object[] skipParameters);
    }
}

