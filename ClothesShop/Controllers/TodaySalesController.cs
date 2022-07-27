using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Helpers;
using ClothesShop.Infrastructure;
using ClothesShop.Models;
using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Authorization = ClothesShop.Infrastructure.Authorization;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class TodaySalesController : BaseController
    {
        private readonly TodayTransactionsRepo _TodayTransactionsRepo;
        public TodaySalesController()
        {
            _TodayTransactionsRepo = new TodayTransactionsRepo();
        }

        [Authorization("TodaySales", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }

        [Authorization("TodaySales", (RoleType.Details))]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodayTransaction todaySales = _TodayTransactionsRepo.GetByID(id.Value);
            if (todaySales == null)
            {
                return HttpNotFound();
            }
            var ex = GetTodayTransactionsViewModel(todaySales);
            return View(ex);
        }

        [Authorization("TodaySales", (RoleType.Add))]
        public ActionResult Create()
        {
            string dateFormat = DateTimeFormatter.DateFormat;
            var current = _TodayTransactionsRepo.GetAll().Where(c => c.CreatedOn.Value.ToString(dateFormat) == DateTime.Now.ToString(dateFormat) && c.IsApproved == false).FirstOrDefault();
            if (current != null)
                return RedirectToAction("Edit", new { id = current.ID });

            TodayTransactionsViewModel model = new TodayTransactionsViewModel()
            {
                Transaction = new TransactionViewModel()
                {
                    ID = 0,
                }
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("TodaySales", (RoleType.Add))]
        public ActionResult Create(TodayTransactionsViewModel todayTransactions)
        {
            //if (todayTransactions.Transactions == null || todayTransactions.Transactions.Count() <= 0)
            //    return View(todayTransactions);

            ModelState.Remove("Transaction.EmployeeID");
            ModelState.Remove("Transaction.ProductID");
            ModelState.Remove("Transaction.NumberOfPieces");
            ModelState.Remove("Transaction.SellingPrice");

            if (ModelState.IsValid)
            {
                TodayTransaction today = GetTodayTransactionModel(todayTransactions, true);
                _TodayTransactionsRepo.Add(today);
                return RedirectToAction("Index");
            }
            return View(todayTransactions);
        }

        [Authorization("TodaySales", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodayTransaction transaction = _TodayTransactionsRepo.GetByID(id.Value);

            if (transaction == null)
            {
                return HttpNotFound();
            }
            var ex = GetTodayTransactionsViewModel(transaction);
            if (transaction.IsApproved.HasValue && transaction.IsApproved.Value)
                return View("Details", ex);

            ex.Transaction = new TransactionViewModel()
            {
                ID = 0
            };
            return View(ex);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("TodaySales", (RoleType.Edit))]
        public ActionResult Edit(TodayTransactionsViewModel todayTransactions)
        {
            //if (todayTransactions.Transactions == null || todayTransactions.Transactions.Count() <= 0)
            //    return View(todayTransactions);
            ModelState.Remove("Transaction.EmployeeID");
            ModelState.Remove("Transaction.ProductID");
            ModelState.Remove("Transaction.NumberOfPieces");
            ModelState.Remove("Transaction.SellingPrice");

            if (ModelState.IsValid)
            {
                var model = GetTodayTransactionModel(todayTransactions);
                _TodayTransactionsRepo.Update(model);
                return RedirectToAction("Index");
            }
            return View(todayTransactions);
        }
        [HttpPost]

        [Authorization("TodaySales", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (!id.HasValue)
                return Json(false);
            return Json(_TodayTransactionsRepo.Delete(id.Value));
        }

        private TodayTransactionsViewModel GetTodayTransactionsViewModel(TodayTransaction t)
        {
            var obj = new TodayTransactionsViewModel()
            {
                ID = t.ID,
                CreatedBy = t.CreatedBy,
                CreatedOn = t.CreatedOn.Value.AddHours(GetUtcOffset()),
                IsApproved = t.IsApproved.Value,
                TodayTotalTransactionsSellingPrice = t.Transactions.Sum(tt => tt.SellingPrice * tt.NumberOfPieces).Value,
                Transactions = t.Transactions.Select(tt => new TransactionViewModel()
                {
                    //EmployeeID = tt.EmployeeID,
                    //EmployeeName = tt.Employee.FullName,
                    ID = tt.ID,
                    //Notes = tt.Notes,
                    NumberOfPieces = tt.NumberOfPieces,
                    ProductID = tt.ProductID,
                    ProductName = tt.Product.Name,
                    SellingPrice = tt.SellingPrice.Value,
                    TodayTransactionID = tt.TodayTransactionID.Value,
                }).ToList(),
            };
            obj.TodaySalesSeralized = JsonConvert.SerializeObject(obj.Transactions);
            return obj;
        }
        private TodayTransaction GetTodayTransactionModel(TodayTransactionsViewModel t, bool isNew = false)
        {
            t.Transactions = JsonConvert.DeserializeObject<List<TransactionViewModel>>(t.TodaySalesSeralized)?.ToList();
            return new TodayTransaction()
            {
                ID = t.ID,
                CreatedBy = isNew ? Session["UserName"].ToString() : t.CreatedBy,
                CreatedOn = isNew ? DateTime.UtcNow : t.CreatedOn,
                IsApproved = t.IsApproved,

                Transactions = t.Transactions.Select(tt => new Transaction()
                {
                    //EmployeeID = tt.EmployeeID,
                    ID = tt.ID,
                    //Notes = tt.Notes,
                    NumberOfPieces = tt.NumberOfPieces,
                    ProductID = tt.ProductID,
                    SellingPrice = tt.SellingPrice,
                    TodayTransactionID = t.ID,
                }).ToList()
            };
        }

        [HttpPost]

        [Authorization("TodaySales", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                int totalRecords = 0;

                Filtering<TodayTransactionsViewModel> filtering = new Filtering<TodayTransactionsViewModel>();
                var dateSearchValue = string.Empty;
                if (obj.FilteredColumns.Count() > 0 && obj.FilteredColumns[0].ColumnName == "CreatedOn_")
                {
                    dateSearchValue = ParseDate(obj.FilteredColumns[0].SearchValue)?.ToString(DateTimeFormatter.DateFormat);
                    obj.FilteredColumns.Remove(obj.FilteredColumns[0]);
                }
                filtering.Columns = obj.FilteredColumns;

                var transactions = _TodayTransactionsRepo.GetAll().Where(c =>
                {
                    DateTime? date = c.CreatedOn.HasValue ? c.CreatedOn.Value.AddHours(GetUtcOffset()) : default;
                    return string.IsNullOrEmpty(dateSearchValue) ||
                    date?.ToString(DateTimeFormatter.DateFormat) == dateSearchValue;
                });

                var result = transactions.Select(n => GetTodayTransactionsViewModel(n));

                //Sorting    
                result = filtering.Search(result);
                if (obj.OrderBy?.ColumnName == "CreatedOn_")
                    obj.OrderBy.ColumnName = "CreatedOn";
                result = filtering.OrderBy(obj.OrderBy, result);

                totalRecords = result.Count();

                var data = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                return Json(new { TotalCount = totalRecords, Data = data });
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return Json(new { TotalCount = 0, Data = string.Empty });
            }
        }

    }
}
