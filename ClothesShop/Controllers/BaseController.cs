using System;
using System.Globalization;
using System.Web.Mvc;
using System.Linq;

namespace ClothesShop.Controllers
{
    public class BaseController : Controller
    {
        internal float GetUtcOffset()
        {
            var defaultOffset = 2;
            try
            {
                bool isParsed = float.TryParse(Session["UtcOffset"]?.ToString(), out float offset);
                return isParsed ? offset : defaultOffset;

            }
            catch
            {
                return defaultOffset;
            }
        }

        internal int GetUserID()
        {
            int empID = 0;
            if (Session["UserID"] != null && !string.IsNullOrEmpty(Session["UserID"].ToString()))
                int.TryParse(Session["UserID"].ToString(), out empID);
            return empID;
        }

        internal int GetJobID()
        {
            int jobId = 4;
            if (Session["JobID"] != null && !string.IsNullOrEmpty(Session["JobID"].ToString()))
                int.TryParse(Session["JobID"].ToString(), out jobId);
            return jobId;

        }

        internal DateTime? ParseDate(string date)
        {
            try
            {
                bool isParsed = DateTime.TryParse(date, out DateTime dateTime);
                if (isParsed)
                    return dateTime;

                date = date.Split(new string[] { " " }, StringSplitOptions.None).FirstOrDefault();
                isParsed = DateTime.TryParseExact(date, "yyyy/MM/dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime);
                if (isParsed)
                    return dateTime;

                date = date.Split(new string[] { "T" }, StringSplitOptions.None).FirstOrDefault();
                isParsed = DateTime.TryParseExact(date, "yyyy/MM/dd", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime);
                if (isParsed)
                    return dateTime;
            }
            catch
            { }

            return null;
        }
    }
}