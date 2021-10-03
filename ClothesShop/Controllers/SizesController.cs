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
    public class SizesController : Controller
    {
        private readonly SizesRepo _SizesRepo;
        public SizesController()
        {
            _SizesRepo = new SizesRepo();
        }
        // GET: Sizes

        [Authorization("Sizes", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Sizes/Create

        [Authorization("Sizes", (RoleType.Add))]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sizes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("Sizes", (RoleType.Add))]
        public ActionResult Create([Bind(Include = "ID,Name")] SizeViewModel size)
        {
            if (ModelState.IsValid)
            {
                Size model = GetSizeModel(size);
                _SizesRepo.Add(model);
                return RedirectToAction("Index");
            }

            return View(size);
        }

        // GET: Sizes/Edit/5

        [Authorization("Sizes", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Size size = _SizesRepo.GetByID(id.Value);
            if (size == null)
            {
                return HttpNotFound();
            }
            var model = GetSizeViewModel(size);
            return View(model);
        }

        // POST: Sizes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorization("Sizes", (RoleType.Edit))]
        public ActionResult Edit([Bind(Include = "ID,Name")] SizeViewModel size)
        {
            if (ModelState.IsValid)
            {
                var model = GetSizeModel(size);
                _SizesRepo.Update(model);
                return RedirectToAction("Index");
            }
            return View(size);
        }

        // GET: Sizes/Delete/5
        [HttpPost]

        [Authorization("Sizes", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (!id.HasValue)
                return Json(false);
            return Json(_SizesRepo.Delete(id.Value));
        }
        private SizeViewModel GetSizeViewModel(Size size)
        {
            return new SizeViewModel()
            {
                ID = size.ID,
                Name = size.Size1
            };
        }
        private Size GetSizeModel(SizeViewModel size)
        {
            return new Size()
            {
                ID = size.ID,
                Size1 = size.Name
            };
        }
        [HttpPost]

        [Authorization("Sizes", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                var sizes = _SizesRepo.GetAll();
                var result = sizes.Select(n => GetSizeViewModel(n));
                Filtering<SizeViewModel> filtering = new Filtering<SizeViewModel>();
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