using System;
using System.Globalization;
using ContractorBackend.Application.Services;

namespace ContractorBackend.Application.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Gets Georgian DateTime in string And Returns DateTime
        /// </summary>
        /// <param name="strToDateTime">Format Must Be yyyy-MM-dd HH:mm:ss</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string strToDateTime)
        {
            if (DateTimeOffset.TryParse(strToDateTime, out var dateTimeoffset))
            {
                var dt = dateTimeoffset.DateTime;
                return dt;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        /// <summary>
        /// Gets Georgian DateOnly in string And Returns DateOnly
        /// </summary>
        /// <param name="strToDateTime">Format Must Be yyyy/mm/dd</param>
        /// <returns></returns>
        public static DateTime ToGeorgianDateOnly(this string strToDateTime)
        {
            PersianCalendar pc = new PersianCalendar();
            var date = Convert.ToDateTime(strToDateTime);
            var dt = new DateTime(date.Year, date.Month, date.Day, pc);
            return Convert.ToDateTime(dt.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets Persian Date Like 140102 OR 14010203 - Gets Gregorian DateTime Standard -> and converts it to 1401/02 OR 1401/02/03
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToPersianDateOnly(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }
            // if string can be converted to DateTime
            if (DateTimeOffset.TryParse(str, out var dateTimeoffset))
            {
                var datetime = dateTimeoffset.DateTime;

                PersianCalendar pc = new PersianCalendar();
                return string.Format("{0}/{1}/{2}", pc.GetYear(datetime), pc.GetMonth(datetime), pc.GetDayOfMonth(datetime));
            }

            if (str.Length == 6)//140210
            {
                return str.Insert(4, "/");

            }
            if (str.Length == 8)//14020506
            {
                return str.Insert(4, "/").Insert(7, "/");
            }
            return string.Empty;
        }

        public static string ToPersianDateTime(this string strToDateTime)
        {
            DateTime datetime;
            if (DateTimeOffset.TryParse(strToDateTime, out var dateTimeoffset))
            {
                datetime = dateTimeoffset.DateTime;
            }
            else
            {
                datetime = DateTime.MinValue;
            }
            PersianCalendar pc = new PersianCalendar();
            return string.Format("{0}/{1}/{2}", pc.GetYear(datetime), pc.GetMonth(datetime), pc.GetDayOfMonth(datetime));
        }

        public static string ToPersianDateTimeDate(this DateTimeOffset? strToDateTime)
        {
            if (strToDateTime != null)
            {
                var date = strToDateTime.Value.DateTime;
                PersianCalendar pc = new PersianCalendar();
                return string.Format("{0}/{1}/{2}", pc.GetYear(date), pc.GetMonth(date), pc.GetDayOfMonth(date));
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strPersianDate">persian date as format yyyy/mm/dd</param>
        /// <returns></returns>
        public static DateTime ToPersianDateTimerFromGregorianDateTime(this string strPersianDate)
        {
            PersianCalendar pc = new PersianCalendar();
            var year = int.Parse(strPersianDate.Substring(0, 4));
            var month = int.Parse(strPersianDate.Substring(5, 2));
            var day = int.Parse(strPersianDate.Substring(8, 2));

            DateTime dt = new DateTime(year, month, day, pc);
            return dt;
        }

        /// <summary>
        /// Convert IsSuite year and month to DateTime
        /// gets IsSuite date like 140302 and convert it to DateTime 
        /// </summary>
        /// <param name="str">string year and month like 140302</param>
        /// <returns></returns>
        public static DateTime ToDateTimeFromIsSuiteYearMonth(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new Exception($"Error in : {nameof(ToDateTimeFromIsSuiteYearMonth)}. the input parameter is null.");
            }
            //140302
            var year = int.Parse(str.Substring(0, 4));
            var month = int.Parse(str.Substring(4, 2));
            //var day = int.Parse(str.Substring(5, 6));

            PersianCalendar pc = new PersianCalendar();
            var newDate = pc.ToDateTime(year, month, 1, 5, 30, 0, 0, PersianCalendar.PersianEra);

            return newDate;
        }



        public static string Decrypt(this string strEncry)
        {
            var strDe = AESService.Decrypt(strEncry);
            return strDe;
        }
        public static string Encrypt(this string strdecry)
        {
            var strEn = AESService.Encrypt(strdecry);
            return strEn;
        }

    }
}
