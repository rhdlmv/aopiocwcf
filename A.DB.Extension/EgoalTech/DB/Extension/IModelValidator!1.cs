namespace EgoalTech.DB.Extension
{
    using System;
    using System.Runtime.InteropServices;

    public interface IModelValidator<T> where T: DbObject, new()
    {
        bool Validate(T model, DbOperation dbOperation = 3);
    }
}

