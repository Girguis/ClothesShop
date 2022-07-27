using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Helpers;
using ClothesShop.Infrastructure;
using ClothesShop.Models;
using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Authorization = ClothesShop.Infrastructure.Authorization;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class TodayExpensController : BaseController
    {
        private readonly TodayExpensesRepo _TodayExpensesRepo;
        public TodayExpensController()
        {
            _TodayExpensesRepo = new TodayExpensesRepo();
        }

        // GET: TodayExpens

        [Authorization("TodayExpens", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }

        // GET: TodayExpens/Details/5

        [Authorization("TodayExpens", (RoleType.Details))]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodayExpens todayExpens = _TodayExpensesRepo.GetByID(id.Value);
            if (todayExpens == null)
            {
                return HttpNotFound();
            }
            var ex = GetTodayExpensViewModel(todayExpens);
            return View(ex);
        }

        // GET: TodayExpens/Create

        [Authorization("TodayExpens", (RoleType.Add))]
        public ActionResult Create()
        {
            string dateFormat = DateTimeFormatter.DateFormat;
            var current = _TodayExpensesRepo.GetAll().Where(c => c.CreatedOn.Value.ToString(dateFormat) == DateTime.UtcNow.ToString(dateFormat) && c.IsApproved == false).FirstOrDefault();
            if (current != null)
                return RedirectToAction("Edit", new { id = current.ID });

            TodayExpenseViewModel model = new TodayExpenseViewModel();
            return View(model);
        }

        // POST: TodayExpens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("TodayExpens", (RoleType.Add))]
        public ActionResult Create(TodayExpenseViewModel todayExpens)
        {
            //if (todayExpens.Expenses == null || todayExpens.Expenses.Count() <= 0)
            //    return View(todayExpens);
            ModelState.Remove("Expense.Name");
            ModelState.Remove("Expense.Cost");
            if (ModelState.IsValid)
            {
                TodayExpens today = GetTodayExpensModel(todayExpens, true);
                _TodayExpensesRepo.Add(today);
                return RedirectToAction("Index");
            }
            return View(todayExpens);
        }

        // GET: TodayExpens/Edit/5

        [Authorization("TodayExpens", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TodayExpens todayExpens = _TodayExpensesRepo.GetByID(id.Value);

            if (todayExpens == null)
            {
                return HttpNotFound();
            }
            var ex = GetTodayExpensViewModel(todayExpens);
            if (todayExpens.IsApproved.HasValue && todayExpens.IsApproved.Value)
                return View("Details", ex);

            return View(ex);
        }

        // POST: TodayExpens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("TodayExpens", (RoleType.Edit))]
        public ActionResult Edit(TodayExpenseViewModel todayExpens)
        {
            //if (todayExpens.Expenses == null || todayExpens.Expenses.Count() <= 0)
            //    return View(todayExpens);
            ModelState.Remove("Expense.Name");
            ModelState.Remove("Expense.Cost");
            if (ModelState.IsValid)
            {
                var model = GetTodayExpensModel(todayExpens);
                _TodayExpensesRepo.Update(model);
                return RedirectToAction("Index");
            }
            return View(todayExpens);
        }
        [HttpPost]

        [Authorization("TodayExpens", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (!id.HasValue)
                return Json(false);
            return Json(_TodayExpensesRepo.Delete(id.Value));
        }

        private TodayExpenseViewModel GetTodayExpensViewModel(TodayExpens e)
        {
            var obj = new TodayExpenseViewModel()
            {
                ID = e.ID,
                IsApproved = e.IsApproved.Value,
                CreatedBy = e.CreatedBy,
                CreatedOn = e.CreatedOn.Value.AddHours(GetUtcOffset()),
                TodayTotalExpensesCost = e.Expenses.Sum(ee => ee.Cost).Value,
                Expenses = e.Expenses.Select(ee => new ExpensesViewModel()
                {
                    ID = ee.ID,
                    Name = ee.Name,
                    Cost = ee.Cost.Value
                }).ToList(),
            };
            obj.TodayExpenseSeralized = JsonConvert.SerializeObject(obj.Expenses);
            return obj;
        }
        private TodayExpens GetTodayExpensModel(TodayExpenseViewModel e, bool isNew = false)
        {
            e.Expenses = JsonConvert.DeserializeObject<List<ExpensesViewModel>>(e.TodayExpenseSeralized)?.ToList();
            return new TodayExpens()
            {
                ID = e.ID,
                CreatedBy = isNew ? Session["UserName"].ToString() : e.CreatedBy,
                CreatedOn = isNew ? DateTime.UtcNow : e.CreatedOn,
                IsApproved = e.IsApproved,
                Expenses = e.Expenses.Select(ee => new Expens()
                {
                    Cost = ee.Cost,
                    Name = ee.Name,
                    ID = ee.ID,
                    TodayExpensesID = ee.TodayExpensesID
                }).ToList(),
            };
        }

        [HttpPost]

        [Authorization("TodayExpens", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                int totalRecords = 0;

                Filtering<TodayExpenseViewModel> filtering = new Filtering<TodayExpenseViewModel>();
                var dateSearchValue = string.Empty;
                if (obj.FilteredColumns.Count() > 0 && obj.FilteredColumns[0].ColumnName == "CreatedOn_")
                {
                    dateSearchValue = ParseDate(obj.FilteredColumns[0].SearchValue)?.ToString(DateTimeFormatter.DateFormat);
                    obj.FilteredColumns.Remove(obj.FilteredColumns[0]);
                }
                filtering.Columns = obj.FilteredColumns;

                var productSuppliers = _TodayExpensesRepo.GetAll().Where(c =>
                {
                    DateTime? date = c.CreatedOn.HasValue ? c.CreatedOn.Value.AddHours(GetUtcOffset()) : default;
                    return string.IsNullOrEmpty(dateSearchValue) ||
                    date?.ToString(DateTimeFormatter.DateFormat) == dateSearchValue;
                });

                var result = productSuppliers.Select(n => GetTodayExpensViewModel(n));
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
