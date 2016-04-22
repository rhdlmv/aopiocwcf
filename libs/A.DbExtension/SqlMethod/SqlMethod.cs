using System;
using System.Collections.Generic;


namespace A.DBExtension.SqlMethod
{
    public static class SqlMethod
    {
        [SqlMethod("ASCII ({0})")]
        public static int Ascii(this string item)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("DEFFERENCE ({0}, {1})")]
        public static int Difference(this string str, string compareStr)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("GETDATE()")]
        public static DateTime GetDate()
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} IN ({1})")]
        public static bool In<T>(this T item, DbQuery subQuery)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} IN ({1})")]
        public static bool In<T>(this T item, DbQuery<T> subQuery)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} IN ({1})")]
        public static bool In<T>(this T item, IEnumerable<T?> list) where T : struct
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} IN ({1})")]
        public static bool In(this string item, IEnumerable<string> list)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} IN ({1})")]
        public static bool In<T>(this T item, IEnumerable<T> list) where T : struct
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} IN ({1})")]
        public static bool In<T>(this T? item, IEnumerable<T?> list) where T : struct
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("( {0} IN ({1}))")]
        public static bool In<T>(this T? item, IEnumerable<T> list) where T : struct
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} IS NOT NULL")]
        public static bool IsNotNull(this string item)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} IS NOT NULL")]
        public static bool IsNotNull<T>(this T item) where T : struct
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} IS NULL")]
        public static bool IsNull(this string item)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} IS NULL")]
        public static bool IsNull<T>(this T item) where T : struct
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("LEFT ({0}, {1})")]
        public static string Left(this string str, int length)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("LEN ({0})")]
        public static int Len(this string str)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} LIKE {1}")]
        public static bool Like(this string str, string likeString)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("LOWER ({0})")]
        public static string Lower(this string str)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("LTRIM ({0})")]
        public static string Ltrim(this string str)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} NOT IN ({1})")]
        public static bool NotIn<T>(this T item, DbQuery subQuery)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} NOT IN ({1})")]
        public static bool NotIn<T>(this T item, DbQuery<T> subQuery)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} NOT IN ({1})")]
        public static bool NotIn<T>(this T item, IEnumerable<T?> list) where T : struct
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} NOT IN ({1})")]
        public static bool NotIn<T>(this T? item, IEnumerable<T?> list) where T : struct
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("( {0} NOT IN ({1}))")]
        public static bool NotIn<T>(this T? item, IEnumerable<T> list) where T : struct
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} NOT IN ({1})")]
        public static bool NotIn(this string item, IEnumerable<string> list)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} NOT IN ({1})")]
        public static bool NotIn<T>(this T item, IEnumerable<T> list) where T : struct
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("{0} NOT LIKE {1}")]
        public static bool NotLike(this string str, string likeString)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("RIGHT ({0}, {1})")]
        public static string Right(this string str, int length)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("RTRIM ({0})")]
        public static string Rtrim(this string str)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }

        [SqlMethod("UPPER ({0})")]
        public static string Upper(this string str)
        {
            throw new NotImplementedException("该方法尚未实现，仅用于辅助生成 SQL 语句。");
        }
    }
}

