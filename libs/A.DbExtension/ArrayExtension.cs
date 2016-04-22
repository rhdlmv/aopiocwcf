namespace A.DBExtension
{
    using System;
    

    public static class ArrayExtension
    {
        public static string Join<T>(this T[] obj, string split)
        {
            if (obj == null)
            {
                return "";
            }
            return string.Join<T>(split, obj);
        }
    }
}

