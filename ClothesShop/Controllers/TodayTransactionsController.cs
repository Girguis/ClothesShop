using System;
using System.Linq;
using System.Web.Mvc;
using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Helpers;
using ClothesShop.Infrastructure;
using ClothesShop.Models;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class TodayTransactionsController : Controller
    {
        private readonly ProductsRepo _ProductsRepo;
        private readonly TodayTransactionsRepo _TodayTransactionsRepo;
        private readonly TodayExpensesRepo _TodayExpensesRepo;
        public TodayTransactionsController()
        {
            _ProductsRepo = new ProductsRepo();
            _TodayTransactionsRepo = new TodayTransactionsRepo();
            _TodayExpensesRepo = new TodayExpensesRepo();
        }
        // GET: TodayTransactions

        [Authorization("TodayTransactions", (RoleType.View))]
        public ActionResult Index()
        {
            string dateFormat = DateTimeFormatter.DateFormat;
            string viewingDateFormat = DateTimeFormatter.ViewingDateFormat;
            ViewBag.StartDate = DateTimeFormatter.GetFirstDayOfWeek(DateTime.Today).ToString(viewingDateFormat);
            ViewBag.EndDate = DateTime.Today.AddDays((int)DayOfWeek.Friday).ToString(viewingDateFormat);

            var transactions = _TodayTransactionsRepo.GetAll();
            var result = transactions.Where(p => p.CreatedOn.Value.ToString(dateFormat) == DateTime.Now.ToString(dateFormat))
                .Select(p => p);

            var todaySales = result.Sum(p => p.Transactions.Sum(t => t.NumberOfPieces * t.SellingPrice)).Value;

            var expenses = _TodayExpensesRepo.GetAll();
            var eResult = expenses.Where(p => p.CreatedOn.Value.ToString(dateFormat) == DateTime.Now.ToString(dateFormat))
                .Select(p => p);
            var todayExpenses = eResult.Sum(p => p.Expenses.Sum(t => t.Cost)).Value;

            ViewBag.TotalTodaySales = todaySales;
            ViewBag.TotalTodayExpenses = todayExpenses;
            ViewBag.TotalTodayRemaining = (todaySales - todayExpenses);
            return View();
        }
        [HttpPost]

        [Authorization("TodayTransactions", (RoleType.View))]
        public ActionResult GetWeekSummary(TransactionsSearchViewModel model)
        {
            try
            {
                var transactions = _TodayTransactionsRepo.GetAll();
                var result = transactions.Where(p => p.CreatedOn >= model.StartDate && p.CreatedOn < model.EndDate.AddDays(1))
                    .Select(p => p);

                var totalSales = result.Sum(p => p.Transactions.Sum(t => t.NumberOfPieces * t.SellingPrice)).Value;

                var expenses = _TodayExpensesRepo.GetAll();
                var eResult = expenses.Where(p => p.CreatedOn >= model.StartDate && p.CreatedOn < model.EndDate.AddDays(1))
                    .Select(p => p);
                var totalExpenses = eResult.Sum(p => p.Expenses.Sum(t => t.Cost)).Value;
                return Json(new { TotalExpenses = totalExpenses, TotalSales = totalSales, TotalRemaining = totalSales - totalExpenses });

            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return Json(new { TotalExpenses = 0, TotalSales = 0, TotalRemaining = 0 });
            }
        }
        [HttpPost]

        [Authorization("TodayTransactions", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj, TransactionsSearchViewModel model)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                string dateFormat = DateTimeFormatter.DateFormat;
                var products = _ProductsRepo.GetAll();
                var result = products.Where(p => p.Transactions.Any(c => c.TodayTransaction.CreatedOn >= model.StartDate && c.TodayTransaction.CreatedOn < model.EndDate.AddDays(1)))
                    .Select(p => new TransactionsViewModel()
                    {
                        ProductName = p.Name,
                        DayOne = p.Transactions.Where(t=>t.TodayTransaction.CreatedOn.Value.ToString(dateFormat) == model.StartDate.AddDays(0).ToString(dateFormat)).Sum(pp=>pp.NumberOfPieces).Value,
                        DayTwo = p.Transactions.Where(t=>t.TodayTransaction.CreatedOn.Value.ToString(dateFormat) == model.StartDate.AddDays(1).ToString(dateFormat)).Sum(pp=>pp.NumberOfPieces).Value,
                        DayThree = p.Transactions.Where(t=>t.TodayTransaction.CreatedOn.Value.ToString(dateFormat) == model.StartDate.AddDays(2).ToString(dateFormat)).Sum(pp=>pp.NumberOfPieces).Value,
                        DayFour = p.Transactions.Where(t=>t.TodayTransaction.CreatedOn.Value.ToString(dateFormat) == model.StartDate.AddDays(3).ToString(dateFormat)).Sum(pp=>pp.NumberOfPieces).Value,
                        DayFive = p.Transactions.Where(t=>t.TodayTransaction.CreatedOn.Value.ToString(dateFormat) == model.StartDate.AddDays(4).ToString(dateFormat)).Sum(pp=>pp.NumberOfPieces).Value,
                        DaySix = p.Transactions.Where(t=>t.TodayTransaction.CreatedOn.Value.ToString(dateFormat) == model.StartDate.AddDays(5).ToString(dateFormat)).Sum(pp=>pp.NumberOfPieces).Value,
                        DaySeven = p.Transactions.Where(t=>t.TodayTransaction.CreatedOn.Value.ToString(dateFormat) == model.StartDate.AddDays(6).ToString(dateFormat)).Sum(pp=>pp.NumberOfPieces).Value,
                    });

                Filtering<TransactionsViewModel> filtering = new Filtering<TransactionsViewModel>();
                filtering.Columns = obj.FilteredColumns;
                //Sorting    
                result = filtering.OrderBy(obj.OrderBy, result);

                int totalRecords = result.Count();

                var data = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                int numberOfPages = (int)(Math.Ceiling(totalRecords / obj.PageSize * 1.0));
              
                return Json(new { NumberOfPages = numberOfPages, data = data });
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return Json(new { NumberOfPages = 0, data = string.Empty });
            }
        }
    }
}