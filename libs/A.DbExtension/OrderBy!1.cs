using A.DB;
using System;
using System.Linq.Expressions;


namespace A.DBExtension
{
    public class OrderBy<T> where T : DbObject, new()
    {
        public Expression<Func<T, object>> Column { get; set; }

        public bool Desc { get; set; }
    }
}

