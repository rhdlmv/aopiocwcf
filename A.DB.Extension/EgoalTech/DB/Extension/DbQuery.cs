﻿namespace EgoalTech.DB.Extension
{
    using System;

    public class DbQuery : IDbQuery
    {
        private string sql;

        public DbQuery()
        {
        }

        public DbQuery(string sql)
        {
            this.sql = sql;
        }

        public virtual string ToSql(ExpressionConverter expressionRouter, params object[] skipParameters) => 
            this.sql
    }
}

