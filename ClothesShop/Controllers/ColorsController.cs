using System;
using System.Data;
using System.Data.Entity;
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
    public class ColorsController : Controller
    {
        private readonly ColorRepo _ColorRepo;
        public ColorsController()
        {
            _ColorRepo = new ColorRepo();
        }
        // GET: Colors
        [Authorization("Colors", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Colors/Create
        [Authorization("Colors", (RoleType.Add))]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Colors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Colors", (RoleType.Add))]
        public ActionResult Create([Bind(Include = "ID,Name")] ColorViewModel color)
        {
            if (ModelState.IsValid)
            {
                Color model = GetColorModel(color);
                _ColorRepo.Add(model);
                return RedirectToAction("Index");
            }

            return View(color);
        }

        // GET: Colors/Edit/5
        [Authorization("Colors", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Color color = _ColorRepo.GetByID(id.Value);
            if (color == null)
            {
                return HttpNotFound();
            }
            var model = GetColorViewModel(color);
            return View(model);
        }

        // POST: Colors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Colors", (RoleType.Edit))]
        public ActionResult Edit([Bind(Include = "ID,Name")] ColorViewModel color)
        {
            if (ModelState.IsValid)
            {
                var model = GetColorModel(color);
                _ColorRepo.Update(model);
                return RedirectToAction("Index");
            }
            return View(color);
        }

        // GET: Colors/Delete/5
        [HttpPost]
        [Authorization("Colors", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (!id.HasValue)
                return Json(false);
            return Json(_ColorRepo.Delete(id.Value));
        }
        private ColorViewModel GetColorViewModel(Color color)
        {
            return new ColorViewModel()
            {
                ID = color.ID,
                Name = color.Color1
            };
        }
        private Color GetColorModel(ColorViewModel color)
        {
            return new Color()
            {
                ID = color.ID,
                Color1 = color.Name
            };
        }

        [HttpPost]
        [Authorization("Colors", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                int totalRecords = 0;

                var colors = _ColorRepo.GetAll();
                var result = colors.Select(n => GetColorViewModel(n));
                Filtering<ColorViewModel> filtering = new Filtering<ColorViewModel>();
                filtering.Columns = obj.FilteredColumns;

                result = filtering.Search("Name", result);

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
    }
}
