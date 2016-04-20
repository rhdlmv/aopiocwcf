namespace EgoalTech.DB.Extension
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public static class DbOperationExtension
    {
        public static string[] ToActions(this DbOperation dbOperation)
        {
            List<string> list = new List<string>();
            if (dbOperation.HasFlag(DbOperation.Create))
            {
                list.Add(DbOperation.Create.ToString());
            }
            if (dbOperation.HasFlag(DbOperation.None | DbOperation.Update))
            {
                list.Add((DbOperation.None | DbOperation.Update).ToString());
            }
            if (dbOperation.HasFlag(DbOperation.Delete))
            {
                list.Add(DbOperation.Delete.ToString());
            }
            return list.ToArray();
        }
    }
}

