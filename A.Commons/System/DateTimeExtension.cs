namespace System
{
    using System.Data.SqlTypes;
    using System.Runtime.CompilerServices;

    public static class DateTimeExtension
    {
        public static string FormatToSql(this DateTime datetime)
        {
            DateTime time = (datetime <= SqlDateTime.MinValue.Value) ? SqlDateTime.MinValue.Value : datetime;
            return $"{time:yyyy-MM-dd HH:mm:ss.fff}";
        }

        public static string FormatToSql(this TimeSpan timeSpan) => 
            $"{DateTime.Now.Date.Add(timeSpan.Duration()):HH:mm:ss.fff}"

        public static string FormatToUTC(this DateTime datetime) => 
            $"{datetime:yyyy-MM-dd HH:mm:ss zz00}"

        public static string FormatToUTC(this DateTime? datetime)
        {
            if (!datetime.HasValue)
            {
                return "null";
            }
            return $"{datetime.Value:yyyy-MM-dd HH:mm:ss zz00}";
        }
    }
}

