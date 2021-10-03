using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.Repositories;

namespace ClothesShop.Areas.Settings.Controllers
{
    public class SystemDataController : Controller
    {
        // GET: Settings/DeleteSystemData
        readonly AccountRepo AccountRepo;
        public SystemDataController()
        {
            AccountRepo = new AccountRepo();
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult DeleteSystemData()
        {
            return Json(AccountRepo.DeleteSystemData());
        }
    }
}