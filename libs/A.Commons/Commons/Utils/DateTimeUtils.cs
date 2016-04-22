namespace A.Commons.Utils
{
    using System;

    public class DateTimeUtils
    {
        public static int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return DateTimeToUnixTimestamp(dateTime, new DateTime(0x7b2, 1, 1));
        }

        public static int DateTimeToUnixTimestamp(DateTime dateTime, DateTime startCountTime)
        {
            TimeSpan span = (TimeSpan) (dateTime - startCountTime.ToLocalTime());
            return (int) span.TotalSeconds;
        }

        public static DateTime GetEndOfCurrentMonth()
        {
            return GetEndOfMonth(GetMonthEnum(DateTime.Now.Month), DateTime.Now.Year);
        }

        public static DateTime GetEndOfCurrentQuarter()
        {
            return GetEndOfQuarter(DateTime.Now.Year, GetQuarter((Month) DateTime.Now.Month));
        }

        public static DateTime GetEndOfCurrentWeek()
        {
            DateTime time = GetStartOfCurrentWeek().AddDays(6.0);
            return new DateTime(time.Year, time.Month, time.Day, 0x17, 0x3b, 0x3b, 0x3e7);
        }

        public static DateTime GetEndOfCurrentYear()
        {
            return GetEndOfYear(DateTime.Now.Year);
        }

        public static DateTime GetEndOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0x17, 0x3b, 0x3b, 0x3e7);
        }

        public static DateTime GetEndOfLastMonth()
        {
            if (DateTime.Now.Month == 1)
            {
                return GetEndOfMonth(Month.December, DateTime.Now.Year - 1);
            }
            return GetEndOfMonth(GetMonthEnum(DateTime.Now.Month - 1), DateTime.Now.Year);
        }

        public static DateTime GetEndOfLastQuarter()
        {
            if (DateTime.Now.Month <= 3)
            {
                return GetEndOfQuarter(DateTime.Now.Year - 1, Quarter.Fourth);
            }
            return GetEndOfQuarter(DateTime.Now.Year, GetQuarter((Month) DateTime.Now.Month));
        }

        public static DateTime GetEndOfLastWeek()
        {
            DateTime time = GetStartOfLastWeek().AddDays(6.0);
            return new DateTime(time.Year, time.Month, time.Day, 0x17, 0x3b, 0x3b, 0x3e7);
        }

        public static DateTime GetEndOfLastYear()
        {
            return GetEndOfYear(DateTime.Now.Year - 1);
        }

        public static DateTime GetEndOfMonth(Month Month, int Year)
        {
            return new DateTime(Year, (int) Month, DateTime.DaysInMonth(Year, (int) Month), 0x17, 0x3b, 0x3b, 0x3e7);
        }

        public static DateTime GetEndOfQuarter(int Year, Quarter Qtr)
        {
            if (Qtr == Quarter.First)
            {
                return new DateTime(Year, 3, DateTime.DaysInMonth(Year, 3), 0x17, 0x3b, 0x3b, 0x3e7);
            }
            if (Qtr == Quarter.Second)
            {
                return new DateTime(Year, 6, DateTime.DaysInMonth(Year, 6), 0x17, 0x3b, 0x3b, 0x3e7);
            }
            if (Qtr == Quarter.Third)
            {
                return new DateTime(Year, 9, DateTime.DaysInMonth(Year, 9), 0x17, 0x3b, 0x3b, 0x3e7);
            }
            return new DateTime(Year, 12, DateTime.DaysInMonth(Year, 12), 0x17, 0x3b, 0x3b, 0x3e7);
        }

        public static DateTime GetEndOfYear(int Year)
        {
            return new DateTime(Year, 12, DateTime.DaysInMonth(Year, 12), 0x17, 0x3b, 0x3b, 0x3e7);
        }

        public static Month GetMonthEnum(int month)
        {
            switch (month)
            {
                case 1:
                    return Month.January;

                case 2:
                    return Month.February;

                case 3:
                    return Month.March;

                case 4:
                    return Month.April;

                case 5:
                    return Month.May;

                case 6:
                    return Month.June;

                case 7:
                    return Month.July;

                case 8:
                    return Month.August;

                case 9:
                    return Month.September;

                case 10:
                    return Month.October;

                case 11:
                    return Month.November;
            }
            return Month.December;
        }

        public static Quarter GetQuarter(Month Month)
        {
            if (Month <= Month.March)
            {
                return Quarter.First;
            }
            if ((Month >= Month.April) && (Month <= Month.June))
            {
                return Quarter.Second;
            }
            if ((Month >= Month.July) && (Month <= Month.September))
            {
                return Quarter.Third;
            }
            return Quarter.Fourth;
        }

        public static DateTime GetStartOfCurrentMonth()
        {
            return GetStartOfMonth(GetMonthEnum(DateTime.Now.Month), DateTime.Now.Year);
        }

        public static DateTime GetStartOfCurrentQuarter()
        {
            return GetStartOfQuarter(DateTime.Now.Year, GetQuarter((Month) DateTime.Now.Month));
        }

        public static DateTime GetStartOfCurrentWeek()
        {
            int dayOfWeek = (int) DateTime.Now.DayOfWeek;
            DateTime time = DateTime.Now.Subtract(TimeSpan.FromDays((double) dayOfWeek));
            return new DateTime(time.Year, time.Month, time.Day, 0, 0, 0, 0);
        }

        public static DateTime GetStartOfCurrentYear()
        {
            return GetStartOfYear(DateTime.Now.Year);
        }

        public static DateTime GetStartOfDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        public static DateTime GetStartOfLastMonth()
        {
            if (DateTime.Now.Month == 1)
            {
                return GetStartOfMonth(Month.December, DateTime.Now.Year - 1);
            }
            return GetStartOfMonth(GetMonthEnum(DateTime.Now.Month - 1), DateTime.Now.Year);
        }

        public static DateTime GetStartOfLastQuarter()
        {
            if (DateTime.Now.Month <= 3)
            {
                return GetStartOfQuarter(DateTime.Now.Year - 1, Quarter.Fourth);
            }
            return GetStartOfQuarter(DateTime.Now.Year, GetQuarter((Month) DateTime.Now.Month));
        }

        public static DateTime GetStartOfLastWeek()
        {
            int num = ((int) DateTime.Now.DayOfWeek) + 7;
            DateTime time = DateTime.Now.Subtract(TimeSpan.FromDays((double) num));
            return new DateTime(time.Year, time.Month, time.Day, 0, 0, 0, 0);
        }

        public static DateTime GetStartOfLastYear()
        {
            return GetStartOfYear(DateTime.Now.Year - 1);
        }

        public static DateTime GetStartOfMonth(Month Month, int Year)
        {
            return new DateTime(Year, (int) Month, 1, 0, 0, 0, 0);
        }

        public static DateTime GetStartOfQuarter(int Year, Quarter Qtr)
        {
            if (Qtr == Quarter.First)
            {
                return new DateTime(Year, 1, 1, 0, 0, 0, 0);
            }
            if (Qtr == Quarter.Second)
            {
                return new DateTime(Year, 4, 1, 0, 0, 0, 0);
            }
            if (Qtr == Quarter.Third)
            {
                return new DateTime(Year, 7, 1, 0, 0, 0, 0);
            }
            return new DateTime(Year, 10, 1, 0, 0, 0, 0);
        }

        public static DateTime GetStartOfYear(int Year)
        {
            return new DateTime(Year, 1, 1, 0, 0, 0, 0);
        }
    }
}

