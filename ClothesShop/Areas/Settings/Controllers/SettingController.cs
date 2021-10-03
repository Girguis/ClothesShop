using System.Web.Mvc;
using ClothesShop.Infrastructure;

namespace ClothesShop.Areas.Settings
{
    [Authentication]
    public class SettingController : Controller
    {
        // GET: Settings/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}
