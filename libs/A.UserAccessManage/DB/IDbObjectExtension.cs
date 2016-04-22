namespace A.DB
{
    using System;

    public interface IDbObjectExtension
    {
        void Load(DbObjectOperator op);
        void Remove(DbObjectOperator op);
        void Save(DbObjectOperator op);
    }
}

