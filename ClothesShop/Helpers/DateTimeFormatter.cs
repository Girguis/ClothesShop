using System;

namespace ClothesShop.Helpers
{
    public static class DateTimeFormatter
    {
        public static string DateFormat { get { return "dd/MM/yyyy"; } }
        public static string MonthDayDateFormat { get { return "MM/dd/yyyy"; } }
        public static string TimeFormat { get { return "hh:mm tt"; } }
        public static string ViewingDateFormat { get { return "yyyy-MM-dd"; } }

        public static DateTime GetFirstDayOfWeek(DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if (diff < 0)
                diff += 7;
            return dt.AddDays(-diff).Date;
        }
    }

}