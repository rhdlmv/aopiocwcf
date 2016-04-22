using A.DB;
using System;
namespace A.DBExtension
{
    public interface IDbObjectInfo<T> where T : DbObject, new()
    {
        void CheckDuplicate(IDbContext context, T model, params string[] fields);
        T CreateClone(IDbContext context, T model);
        string FixCondition(string condition);
        DateTime GetModifyDatetime(T model);
        F GetPrimaryKey<F>(T model);
        T ImportClone(IDbContext context, T model);
        T RemoveClone(IDbContext context, T model);
        T SetLogicalDelete(T model);
        T SetModifyDatetime(T model);
        T UpdateClone(IDbContext context, T model, params string[] updateFields);

        bool IsDbLog { get; }

        bool IsLogicalDelete { get; }
    }
}

