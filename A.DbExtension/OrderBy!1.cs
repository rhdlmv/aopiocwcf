namespace A.DBExtension
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public class OrderBy<T> where T: DbObject, new()
    {
        public Expression<Func<T, object>> Column { get; set; }

        public bool Desc { get; set; }
    }
}

