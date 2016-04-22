using A.DB;
using System;
using System.Runtime.InteropServices;

namespace A.DBExtension
{
    public interface IModelValidator<T> where T : DbObject, new()
    {
        bool Validate(T model, DbOperation dbOperation = DbOperation.CU);
    }
}

