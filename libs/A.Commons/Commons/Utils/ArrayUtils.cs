namespace A.Commons.Utils
{
    using System;
    using System.Collections.Generic;

    public class ArrayUtils
    {
        public static bool IsEqual<T>(T[] a1, T[] a2)
        {
            if (!object.ReferenceEquals(a1, a2))
            {
                if ((a1 == null) || (a2 == null))
                {
                    return false;
                }
                if (a1.Length != a2.Length)
                {
                    return false;
                }
                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                for (int i = 0; i < a1.Length; i++)
                {
                    if (!comparer.Equals(a1[i], a2[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static T[] Merge<T>(params T[][] arrays)
        {
            List<T> list = new List<T>();
            foreach (T[] localArray in arrays)
            {
                if (localArray != null)
                {
                    foreach (T local in localArray)
                    {
                        list.Add(local);
                    }
                }
            }
            return list.ToArray();
        }

        public static object[] ToObjectArray(params object[] objects)
        {
            List<object> list = new List<object>();
            foreach (object obj2 in objects)
            {
                if (obj2 == null)
                {
                    list.Add(null);
                }
                else if (obj2.GetType().IsArray)
                {
                    foreach (object obj3 in (Array) obj2)
                    {
                        list.Add(obj3);
                    }
                }
                else
                {
                    list.Add(obj2);
                }
            }
            return list.ToArray();
        }
    }
}

