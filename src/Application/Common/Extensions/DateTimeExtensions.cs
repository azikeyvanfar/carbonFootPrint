using System;
using System.Globalization;

namespace ContractorBackend.Common.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// gets DateTime and convert to Is-suite Date format like 14030508
        /// </summary>
        /// <param name="date">Datetime value</param>
        /// <returns></returns>
        public static string ToIsSuiteDateTime(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            return string.Format("{0}{1}{2}", pc.GetYear(date), pc.GetMonth(date), pc.GetDayOfMonth(date));
        }

        public static DateTime GetFirstPersianDayOfWeek(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            var Year = pc.GetYear(date);
            var Month = pc.GetMonth(date);
            var day = pc.GetDayOfMonth(date);
            var firstDayOfWeek = new CultureInfo("fa-IR").DateTimeFormat.FirstDayOfWeek;
            int diff = (7 + date.DayOfWeek - firstDayOfWeek) % 7;

            DateTime firstDay = new DateTime(Year, Month, day, 0, 0, 0, pc).AddDays(-diff);

            return firstDay;
        }
        public static DateTime GetFirstPersianDayOfMonth(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            var Year = pc.GetYear(date);
            var Month = pc.GetMonth(date);
            DateTime firstDay = new DateTime(Year, Month, 1, 0, 0, 0, pc);

            return firstDay;
        }
        public static DateTime GetFirstPersianDayOfYear(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            var Year = pc.GetYear(date);
            DateTime firstDay = new DateTime(Year, 1, 1, 0, 0, 0, pc);

            return firstDay;
        }

        public static DateTime GetLastPersianDayOfMonth(this DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();

            var daysInMonth = pc.GetDaysInMonth(pc.GetYear(date), pc.GetMonth(date));
            DateTime lastDay = new DateTime(pc.GetYear(date), pc.GetMonth(date), daysInMonth, 23, 59, 59, 59, pc);

            return lastDay;
        }

        public static DateTime GetFirstGregorianDayOfMonth(this DateTime date)
        {
            DateTime firstDay = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
            return firstDay;
        }

        public static DateTime GetLastGregorianDayOfMonth(this DateTime date)
        {
            DateTime lastDay = new DateTime(date.Year, date.Month, 1, 23, 59, 59).AddMonths(1).AddDays(-1);
            return lastDay;
        }

        public static string GetPersianYearAndMonth(this DateTime date, string separator)
        {
            PersianCalendar pc = new PersianCalendar();
            return $"{pc.GetYear(date)}{separator}{pc.GetMonth(date)}";
        }

        public static string GetPersianYearAndMonthAndDay(this DateTime date, string separator)
        {
            PersianCalendar pc = new PersianCalendar();
            return $"{pc.GetYear(date)}{separator}{pc.GetMonth(date)}{separator}{pc.GetDayOfMonth(date).ToString("00")}";
        }


    }
}
