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
    public class ShipmentCompaniesController : Controller
    {
        private readonly ShipmentCompanyRepo _ShipmentCompanysRepo;
        public ShipmentCompaniesController()
        {
            _ShipmentCompanysRepo = new ShipmentCompanyRepo();
        }
        // GET: ShipmentCompanys

        [Authorization("ShipmentCompanies", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }

        // GET: ShipmentCompanys/Create

        [Authorization("ShipmentCompanies", (RoleType.Add))]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShipmentCompanys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("ShipmentCompanies", (RoleType.Add))]
        public ActionResult Create(ShipmentCompanyViewModel shipmentCompany)
        {
            if (ModelState.IsValid)
            {
                ShipmentCompany model = GetShipmentCompanyModel(shipmentCompany);
                _ShipmentCompanysRepo.Add(model);
                return RedirectToAction("Index");
            }

            return View(shipmentCompany);
        }

        // GET: ShipmentCompanys/Edit/5

        [Authorization("ShipmentCompanies", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShipmentCompany ShipmentCompany = _ShipmentCompanysRepo.GetByID(id.Value);
            if (ShipmentCompany == null)
            {
                return HttpNotFound();
            }
            var model = GetShipmentCompanyViewModel(ShipmentCompany);
            return View(model);
        }

        // POST: ShipmentCompanys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("ShipmentCompanies", (RoleType.Edit))]
        public ActionResult Edit(ShipmentCompanyViewModel ShipmentCompany)
        {
            if (ModelState.IsValid)
            {
                var model = GetShipmentCompanyModel(ShipmentCompany);
                _ShipmentCompanysRepo.Update(model);
                return RedirectToAction("Index");
            }
            return View(ShipmentCompany);
        }

        // GET: ShipmentCompanys/Delete/5
        [HttpPost]

        [Authorization("ShipmentCompanies", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (!id.HasValue)
                return Json(false);
            return Json(_ShipmentCompanysRepo.Delete(id.Value));
        }
        private ShipmentCompanyViewModel GetShipmentCompanyViewModel(ShipmentCompany c)
        {
            return new ShipmentCompanyViewModel()
            {
                ID = c.ID,
                Name = c.Name,
                Address = c.Address,
                MobileNumber = c.MobileNumber
            };
        }
        private ShipmentCompany GetShipmentCompanyModel(ShipmentCompanyViewModel c)
        {
            return new ShipmentCompany()
            {
                ID = c.ID,
                Name = c.Name,
                Address = c.Address,
                MobileNumber = c.MobileNumber
            };
        }
        [HttpPost]
        [Authorization("ShipmentCompanies", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                var sizes = _ShipmentCompanysRepo.GetAll();
                var result = sizes.Select(n => GetShipmentCompanyViewModel(n));
                Filtering<ShipmentCompanyViewModel> filtering = new Filtering<ShipmentCompanyViewModel>();
                filtering.Columns = obj.FilteredColumns;

                result = filtering.Search(result);

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