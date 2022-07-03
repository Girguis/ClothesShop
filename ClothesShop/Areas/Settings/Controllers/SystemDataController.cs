using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Infrastructure;

namespace ClothesShop.Areas.Settings.Controllers
{
    [Authentication]
    public class SystemDataController : Controller
    {

        // GET: Settings/DeleteSystemData
        readonly AccountRepo AccountRepo;
        public SystemDataController()
        {
            AccountRepo = new AccountRepo();
        }

        [Authorization("Logins", (RoleType.Delete))]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Authorization("Logins", (RoleType.Delete))]
        public JsonResult DeleteSystemData()
        {
            return Json(AccountRepo.DeleteSystemData());
        }
    }
}