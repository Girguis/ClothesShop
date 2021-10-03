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
    public class SuppliersController : Controller
    {
        private readonly SuppliersRepo _SuppliersRepo;
        public SuppliersController()
        {
            _SuppliersRepo = new SuppliersRepo();
        }
        // GET: Suppliers

        [Authorization("Suppliers", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Suppliers/Create

        [Authorization("Suppliers", (RoleType.Add))]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("Suppliers", (RoleType.Add))]
        public ActionResult Create(SupplierViewModel supplier)
        {
            if (ModelState.IsValid)
            {
                Supplier model = GetSupplierModel(supplier);
                _SuppliersRepo.Add(model);
                return RedirectToAction("Index");
            }

            return View(supplier);
        }

        // GET: Suppliers/Edit/5

        [Authorization("Suppliers", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = _SuppliersRepo.GetByID(id.Value);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            var model = GetSupplierViewModel(supplier);
            return View(model);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("Suppliers", (RoleType.Edit))]
        public ActionResult Edit(SupplierViewModel supplier)
        {
            if (ModelState.IsValid)
            {
                var model = GetSupplierModel(supplier);
                _SuppliersRepo.Update(model);
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        [HttpPost]

        [Authorization("Suppliers", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (!id.HasValue)
                return Json(false);
            return Json(_SuppliersRepo.Delete(id.Value));
        }
        private SupplierViewModel GetSupplierViewModel(Supplier s)
        {
            return new SupplierViewModel()
            {
                ID = s.ID,
                MobileNumber1 = s.MobileNumber1,
                MobileNumber2 = s.MobileNumber2,
                Name = s.Name
            };
        }
        private Supplier GetSupplierModel(SupplierViewModel s)
        {
            return new Supplier()
            {
                ID = s.ID,
                MobileNumber1 = s.MobileNumber1,
                MobileNumber2 = s.MobileNumber2,
                Name = s.Name
            };
        }
        [HttpPost]
        [Authorization("Suppliers", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                var sizes = _SuppliersRepo.GetAll();
                var result = sizes.Select(n => GetSupplierViewModel(n));
                Filtering<SupplierViewModel> filtering = new Filtering<SupplierViewModel>();
                filtering.Columns = obj.FilteredColumns;

                result = filtering.Search(result);
               
                //Sorting    
                result = filtering.OrderBy(obj.OrderBy, result);

                int totalRecords = result.Count();
                var data = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                int numberOfPages = (int)(Math.Ceiling(totalRecords * 1.0 / pageSize));

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