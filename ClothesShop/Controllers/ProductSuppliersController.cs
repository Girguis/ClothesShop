using System;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Resources;
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
    public class ProductSuppliersController : BaseController
    {
        private readonly ProductSuppliersRepo _ProductSuppliersRepo;
        public ProductSuppliersController()
        {
            _ProductSuppliersRepo = new ProductSuppliersRepo();
        }

        // GET: ProductSuppliers
        [Authorization("ProductSuppliers", (RoleType.View))]

        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductSuppliers/Details/5

        [Authorization("ProductSuppliers", (RoleType.Details))]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSupplier obj = _ProductSuppliersRepo.GetByID(id.Value);
            var productSupplier = GetProductSupllierViewModel(obj);
            if (productSupplier == null)
            {
                return HttpNotFound();
            }
            return View(productSupplier);
        }

        // GET: ProductSuppliers/Create

        [Authorization("ProductSuppliers", (RoleType.Add))]
        public ActionResult Create()
        {
            var model = new ProductSuppliersViewModel();
            return View(model);
        }

        // POST: ProductSuppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("ProductSuppliers", (RoleType.Add))]
        public ActionResult Create(ProductSuppliersViewModel productSupplier)
        {
            if (ModelState.IsValid)
            {
                var obj = GetProductSupllierModel(productSupplier, true);
                _ProductSuppliersRepo.Add(obj);
                return RedirectToAction("Index");
            }
            return View(productSupplier);
        }

        // GET: ProductSuppliers/Edit/5

        [Authorization("ProductSuppliers", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductSupplier obj = _ProductSuppliersRepo.GetByID(id.Value);
            var productSupplier = GetProductSupllierViewModel(obj);
            if (productSupplier == null)
            {
                return HttpNotFound();
            }
            return View(productSupplier);
        }

        // POST: ProductSuppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("ProductSuppliers", (RoleType.Edit))]
        public ActionResult Edit(ProductSuppliersViewModel productSupplier)
        {
            if (ModelState.IsValid)
            {
                var obj = GetProductSupllierModel(productSupplier);
                _ProductSuppliersRepo.Update(obj);
                return RedirectToAction("Index");
            }
            return View(productSupplier);
        }


        // GET: ProductSuppliers/Delete/5
        [HttpPost]

        [Authorization("ProductSuppliers", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (!id.HasValue)
                return Json(false);
            return Json(_ProductSuppliersRepo.Delete(id.Value));
        }

        [HttpPost]

        [Authorization("ProductSuppliers", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                int totalRecords = 0;
                var productSuppliers = _ProductSuppliersRepo.GetAll();
                var result = productSuppliers.Select(n => GetProductSupllierViewModel(n));
                Filtering<ProductSuppliersViewModel> filtering = new Filtering<ProductSuppliersViewModel>();

                var dateSearchValue = string.Empty;
                if(obj.FilteredColumns.Count() > 0 && obj.FilteredColumns[0].ColumnName == "CreatedOn_")
                {
                    var splittedDate = obj.FilteredColumns[0].SearchValue.Split(new string[] { " " }, StringSplitOptions.None).FirstOrDefault();
                    dateSearchValue = DateTime.ParseExact(splittedDate, "yyyy/MM/dd", CultureInfo.CurrentCulture).ToString("dd/MM/yyyy");
                    obj.FilteredColumns.Remove(obj.FilteredColumns[0]);
                }
                filtering.Columns = obj.FilteredColumns;

                result = filtering.Search(result);
                if(!string.IsNullOrEmpty(dateSearchValue))
                    result = result.Where(d => d.CreatedOn_.Split(new string[] { " " }, StringSplitOptions.None).FirstOrDefault() == dateSearchValue);
                //Sorting    
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
        private ProductSuppliersViewModel GetProductSupllierViewModel(ProductSupplier productSupplier)
        {
            var transactionTypeName = new ResourceManager(typeof(Languages.Resources))
                .GetString(Enum.GetName(typeof(TransactionTypes), productSupplier.TransactionTypeID), CultureInfo.CurrentCulture);
            return new ProductSuppliersViewModel()
            {
                CreatedBy = productSupplier.CreatedBy,
                CreatedOn = productSupplier.CreatedOn.Value.AddHours(GetUtcOffset()),
                ID = productSupplier.ID,
                NumberOfPieces = productSupplier.NumberOfPieces,
                OrginalPrice = productSupplier.OrginalPrice,
                ProductID = productSupplier.ProductID,
                SupplierID = productSupplier.SupplierID,
                TransactionTypeID = productSupplier.TransactionTypeID,
                ProductName = productSupplier.Product.Name,
                SupplierName = productSupplier.Supplier.Name,
                TransactionName = transactionTypeName,
            };
        }
        private ProductSupplier GetProductSupllierModel(ProductSuppliersViewModel productSupplier, bool isNew = false)
        {
            return new ProductSupplier()
            {
                CreatedBy = !isNew ? productSupplier.CreatedBy : Session["UserName"].ToString(),
                CreatedOn = !isNew ? productSupplier.CreatedOn : DateTime.UtcNow,
                ID = productSupplier.ID,
                NumberOfPieces = productSupplier.NumberOfPieces,
                OrginalPrice = productSupplier.OrginalPrice,
                ProductID = productSupplier.ProductID,
                SupplierID = productSupplier.SupplierID,
                TransactionTypeID = productSupplier.TransactionTypeID
            };
        }
    }

}
