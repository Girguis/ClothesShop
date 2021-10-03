using System;
using System.Linq;
using System.Web.Mvc;
using BLL.Repositories;
using ClothesShop.Helpers;
using ClothesShop.Infrastructure;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class StatisticsController : Controller
    {
        private readonly TodayTransactionsRepo _TodayTransactionsRepo;
        private readonly SalesRatesRepo _SalesRatesRepo;
        public StatisticsController()
        {
            _TodayTransactionsRepo = new TodayTransactionsRepo();
            _SalesRatesRepo = new SalesRatesRepo();
        }
        [Authorization("WeeklyStatistics", Enums.RoleType.View)]
        public ActionResult Weekly()
        {
            string viewingDateFormat = DateTimeFormatter.ViewingDateFormat;
            ViewBag.StartDate = DateTimeFormatter.GetFirstDayOfWeek(DateTime.Today).ToString(viewingDateFormat);
            ViewBag.EndDate = DateTime.Today.ToString(viewingDateFormat);
            return View();
        }
        [Authorization("MonthlyStatistics", Enums.RoleType.View)]
        public ActionResult Monthly()
        {
            string viewingDateFormat = DateTimeFormatter.ViewingDateFormat;
            ViewBag.StartDate = new DateTime(DateTime.Today.Year , DateTime.Today.Month , 1).ToString(viewingDateFormat);
            ViewBag.EndDate = DateTime.Today.ToString(viewingDateFormat);
            return View();
        }
        [HttpPost]
        public ActionResult GetStatistics(DateTime start, DateTime end)
        {
            try
            {
                double days = (end - start).TotalDays + 1;
                var data = _TodayTransactionsRepo.GetAll().Where(c => c.CreatedOn >= start && c.CreatedOn < end.AddDays(1));

                var result = data.GroupBy(c => c.CreatedOn.Value.ToString(DateTimeFormatter.DateFormat) ,
                    (key , value) => new 
                    { 
                        CreatedOn_ = key , 
                        CreatedDate = value.First().CreatedOn.HasValue? value.First().CreatedOn.Value.ToString(DateTimeFormatter.MonthDayDateFormat):"", 
                        TotalTransactions =  value.Sum(x=>x.Transactions.Sum(cv=>cv.NumberOfPieces * cv.SellingPrice))
                    }).OrderBy(o=>DateTime.Parse(o.CreatedOn_));

                var paymentAverage = (result.Sum(r => r.TotalTransactions) / days);

                var salesRates = _SalesRatesRepo.GetAll();

                var rate = (salesRates != null && salesRates.Count() > 0) ?
                    salesRates.Where(r => (r.From.HasValue && r.To.HasValue && r.From <= paymentAverage && r.To >= paymentAverage) || 
                    (!r.From.HasValue && r.To >= paymentAverage) || (!r.To.HasValue && r.From <= paymentAverage)).FirstOrDefault() : null;


                return Json(new { data = result , PaymentAverage = paymentAverage  , Rate = (rate != null ? rate.Name :"") , RateColor = (rate != null ? rate.RateColor : "") });
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return Json(new { data = string.Empty, PaymentAverage = 0, Rate = "" });
            }
        }
    }
}