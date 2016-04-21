namespace System
{
    using System.Data.SqlTypes;
    using System.Runtime.CompilerServices;

    public static class DateTimeExtension
    {
        public static string FormatToSql(this DateTime datetime)
        {
            DateTime time = (datetime <= SqlDateTime.MinValue.Value) ? SqlDateTime.MinValue.Value : datetime;
            return string.Format("{0:yyyy-MM-dd HH:mm:ss.fff}", time);
        }

        public static string FormatToSql(this TimeSpan timeSpan)
        {
            return string.Format("{0:HH:mm:ss.fff}", DateTime.Now.Date.Add(timeSpan.Duration()));
        }

        public static string FormatToUTC(this DateTime datetime)
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss zz00}", datetime);
        }

        public static string FormatToUTC(this DateTime? datetime)
        {
            if (!datetime.HasValue)
            {
                return "null";
            }
            return string.Format("{0:yyyy-MM-dd HH:mm:ss zz00}", datetime.Value);
        }
    }
}

