namespace A.DBExtension
{
    using System;
    using System.Runtime.InteropServices;

    public interface IModelValidator<T> where T: DbObject, new()
    {
        bool Validate(T model, DbOperation dbOperation = 3);
    }
}

