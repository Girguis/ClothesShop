using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using BLL.Repositories;
using ClothesShop.Languages;
using ClothesShop.Models;
using DAL;
using Logging.Enums;
using Logging.Services;

namespace ClothesShop.Controllers
{
    public class AccountController : Controller
    {
        readonly AccountRepo AccountRepo;


        public AccountController()
        {
            AccountRepo = new AccountRepo();
        }

        [HttpGet]
        // GET: Login
        public ActionResult Index()
        {
            string lang = WebConfigurationManager.AppSettings["Language"];
            ResourcesManager.SetLanguage(lang);

            if (Session["UserName"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                string hashedPassword = model.Password;
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                    hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }
                model.Password = hashedPassword;
                Employee user = new Employee();
                Login _model = new Login()
                {
                    UserName = model.UserName,
                    Password = hashedPassword,
                };
                bool isExists = AccountRepo.IsExists(_model);
                if (isExists)
                {
                    var login = AccountRepo.Get(_model);
                    string lang = WebConfigurationManager.AppSettings["Language"];
                    ResourcesManager.SetLanguage(lang);
                    Session["UserName"] = model.UserName;
                    Session["UserID"] = login.EmployeeID;
                    JobRolesRepo _JobTypesRepo = new JobRolesRepo();
                    List<JobRole>jobRoles = _JobTypesRepo.GetByJobID(login.Employee.JobTypeID).ToList();
                    Session["Roles"] = jobRoles;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", Resources.InvalidUserNameOrPassword);
                    var obj = new
                    {
                        Msg = "InvalidEmailOrPassword",
                        User = model
                    };
                    return View("Index", model);
                }
            }
            catch (Exception ex)
            {
                LogErrorService.Write(AppTypes.PresentationLayer, ex);
                ModelState.AddModelError("", Resources.ErrorWhileLogin);
                return View("Error");
            }
        }
        public ActionResult Logout()
        {
            Session["UserName"] = null;
            Session["UserID"] = null;
            Session["Roles"] = null;
            return RedirectToAction("Index", "Account");
        }
        [AllowAnonymous]
        public bool IsUserNameExists(long EmployeeID,string UserName)
        {
            try
            {
                return AccountRepo.IsUserNameExists(EmployeeID,UserName);
            }
            catch(Exception ex)
            {
                Logging.Services.LogErrorService.Write(AppTypes.PresentationLayer, ex);
                return true;
            }
        }
    }
}