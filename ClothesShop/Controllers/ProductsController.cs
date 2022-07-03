using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Helpers;
using ClothesShop.Infrastructure;
using ClothesShop.Models;
using DAL;
using Authorization = ClothesShop.Infrastructure.Authorization;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class ProductsController : Controller
    {
        private readonly ProductsRepo _ProductsRepo;
        public ProductsController()
        {
            _ProductsRepo = new ProductsRepo();
        }
        // GET: Products
        [Authorization("Products", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }
        // GET: Products/Create
        [Authorization("Products", (RoleType.Add))]
        public ActionResult Create()
        {
            var model = new ProductViewModel();
            return View(model);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Products", (RoleType.Add))]
        public ActionResult Create(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                Product product = GetProductModel(productViewModel);
                _ProductsRepo.Add(product);
                return RedirectToAction("Index");
            }

            return View(productViewModel);
        }

        // GET: Products/Edit/5
        [Authorization("Products", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var product = _ProductsRepo.GetByID(id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }
            ProductViewModel productViewModel = new ProductViewModel()
            {
                ID = product.ID,
                Name = product.Name,
                OriginalPrice = product.OrginalPrice.Value,
                ColorIDs = product.ProductColors.Select(p => p.ColorID.Value.ToString()).ToList(),
                SizeIDs = product.ProductSizes.Select(p => p.SizeID.Value.ToString()).ToList(),
            };

            return View(productViewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Products", (RoleType.Edit))]
        public ActionResult Edit(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                Product product = GetProductModel(productViewModel);
                _ProductsRepo.Update(product);
                return RedirectToAction("Index");
            }
            return View(productViewModel);
        }

        // GET: Products/Delete/5
        [HttpPost]
        [Authorization("Products", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (!id.HasValue)
                return Json(false);
            return Json(_ProductsRepo.Delete(id.Value));
        }

        [HttpPost]
        public JsonResult GetProductInfo(long? productId)
        {
            List<General> colors = new List<General>();
            List<General> sizes = new List<General>();
            try
            {
                if(productId.HasValue)
                {
                    var product = _ProductsRepo.GetByID(productId.Value);
                    colors = product.ProductColors.Select(p => new General()
                    {
                        ID = p.ColorID.Value,
                        Name = p.Color.Color1
                    }).ToList();
                    
                    sizes = product.ProductSizes.Select(p => new General()
                    {
                        ID = p.SizeID.Value,
                        Name = p.Size.Size1
                    }).ToList();
                }
            }
            catch(Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return Json(new { Sizes = sizes, Colors = colors });
        }
        private Product GetProductModel(ProductViewModel model)
        {
            return new Product()
            {
                ID = model.ID,
                Name = model.Name,
                OrginalPrice = model.OriginalPrice,
                ProductColors = model.ColorIDs.Select(p => new ProductColor() { ProductID = model.ID, ColorID = int.Parse(p) }).ToList(),
                ProductSizes = model.SizeIDs.Select(p => new ProductSize() { ProductID = model.ID, SizeID = int.Parse(p) }).ToList(),
                IsEnabled = true,
            };
        }
        private ProductViewModel GetProductViewModel(Product p)
        {
            return new ProductViewModel()
            {
                ID = p.ID,
                Name = p.Name,
                OriginalPrice = p.OrginalPrice.Value,
                ColorNames = p.ProductColors.Select(pc => pc.Color.Color1).ToList(),
                SizeNames = p.ProductSizes.Select(pc => pc.Size.Size1).ToList(),
            };
        }
        [HttpPost]

        [Authorization("Products", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                var sizes = _ProductsRepo.GetAll();
                var result = sizes.Select(n => GetProductViewModel(n));
                Filtering<ProductViewModel> filtering = new Filtering<ProductViewModel>();
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
