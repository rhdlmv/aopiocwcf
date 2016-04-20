namespace EgoalTech.DB.Extension
{
    using System;
    using System.Runtime.CompilerServices;

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

