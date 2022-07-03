using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Infrastructure;
using ClothesShop.Models;
using DAL;
using Authorization = ClothesShop.Infrastructure.Authorization;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class SalesRatesController : Controller
    {
        private readonly SalesRatesRepo _SalesRateRepo;
        public SalesRatesController()
        {
            _SalesRateRepo = new SalesRatesRepo();
        }
        // GET: SalesRates

        [Authorization("SalesRates", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }

        // GET: SalesRates/Create

        [Authorization("SalesRates", (RoleType.Add))]
        public ActionResult Create()
        {
            var model = new SalesRateViewModel();
            return View(model);
        }

        // POST: SalesRates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("SalesRates", (RoleType.Add))]
        public ActionResult Create(SalesRateViewModel SalesRate)
        {
            if(SalesRate.From.HasValue && !SalesRate.To.HasValue)
                ModelState.Remove("To");
            
            if(!SalesRate.From.HasValue && SalesRate.To.HasValue)
                ModelState.Remove("From");
            
            if (ModelState.IsValid)
            {
                SalesRate model = GetSalesRateModel(SalesRate);
                _SalesRateRepo.Add(model);
                return RedirectToAction("Index");
            }

            return View(SalesRate);
        }

        // GET: SalesRates/Edit/5

        [Authorization("SalesRates", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesRate SalesRate = _SalesRateRepo.GetByID(id.Value);
            if (SalesRate == null)
            {
                return HttpNotFound();
            }
            var model = GetSalesRateViewModel(SalesRate);
            return View(model);
        }

        // POST: SalesRates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("SalesRates", (RoleType.Edit))]
        public ActionResult Edit(SalesRateViewModel SalesRate)
        {
            if (SalesRate.From.HasValue && !SalesRate.To.HasValue)
                ModelState.Remove("To");

            if (!SalesRate.From.HasValue && SalesRate.To.HasValue)
                ModelState.Remove("From");

            if (ModelState.IsValid)
            {
                var model = GetSalesRateModel(SalesRate);
                _SalesRateRepo.Update(model);
                return RedirectToAction("Index");
            }
            return View(SalesRate);
        }

        // GET: SalesRates/Delete/5
        [HttpPost]

        [Authorization("SalesRates", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (!id.HasValue)
                return Json(false);
            return Json(_SalesRateRepo.Delete(id.Value));
        }
        private SalesRateViewModel GetSalesRateViewModel(SalesRate rate)
        {
            return new SalesRateViewModel()
            {
                ID = rate.ID,
                Name = rate.Name,
                From = rate.From,
                To = rate.To,
                RateColor = rate.RateColor
            };
        }
        private SalesRate GetSalesRateModel(SalesRateViewModel rate)
        {
            return new SalesRate()
            {
                ID = rate.ID,
                Name = rate.Name,
                From = rate.From,
                To = rate.To,
                RateColor = rate.RateColor
            };
        }
        [HttpPost]

        [Authorization("SalesRates", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                var SalesRates = _SalesRateRepo.GetAll();
                var result = SalesRates.Select(n => GetSalesRateViewModel(n));
                Filtering<SalesRateViewModel> filtering = new Filtering<SalesRateViewModel>();
                filtering.Columns = obj.FilteredColumns;

                result = filtering.Search("Name", result);

                //Sorting    
                result = filtering.OrderBy(obj.OrderBy, result);

                int totalRecords = result.Count();
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