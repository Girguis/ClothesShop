using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Infrastructure;
using ClothesShop.Models;
using DAL;
using Authorization = ClothesShop.Infrastructure.Authorization;
using LoginViewModel = ClothesShop.Areas.Settings.Models.LoginViewModel;

namespace ClothesShop.Areas.Settings.Controllers
{
    [Authentication]
    public class LoginsController : Controller
    {
        private readonly AccountRepo _AccountRepo;
        public LoginsController()
        {
            _AccountRepo = new AccountRepo();
        }

        // GET: Settings/Logins
        [Authorization("Logins", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }
        // GET: Settings/Logins/Create
        [Authorization("Logins", (RoleType.Add))]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Settings/Logins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Logins", (RoleType.Add))]
        public ActionResult Create(LoginViewModel login)
        {
            ModelState.Remove("ID");
            if (ModelState.IsValid)
            {
                AccountRepo accountRepo = new AccountRepo();
                bool isExists = accountRepo.IsUserNameExists(login.EmployeeID, login.UserName);
                if (isExists)
                {
                    ModelState.AddModelError("UserName", Languages.Resources.UserNameAlreadyExists);
                }
                else
                {
                    var model = GetLoginModel(login);
                    _AccountRepo.Add(model);
                    return RedirectToAction("Index");
                }

            }
            return View(login);
        }

        // GET: Settings/Logins/Edit/5
        [Authorization("Logins", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Login login = _AccountRepo.GetByID(id.Value);
            LoginViewModel model = GetLoginViewModel(login);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: Settings/Logins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Logins", (RoleType.Edit))]
        public ActionResult Edit(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                AccountRepo accountRepo = new AccountRepo();
                bool isExists = accountRepo.IsUserNameExists(login.EmployeeID, login.UserName);
                if (isExists)
                {
                    ModelState.AddModelError("UserName", Languages.Resources.UserNameAlreadyExists);
                }
                else
                {
                    var model = GetLoginModel(login);
                    _AccountRepo.Update(model);
                    return RedirectToAction("Index");
                }
            }
            return View(login);
        }

        // GET: Settings/Logins/Delete/5
        [HttpPost]
        [Authorization("Logins", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (id == null)
            {
                return Json(false);
            }
            bool isDeleted = _AccountRepo.Delete(id.Value);
            return Json(isDeleted);
        }

        private Login GetLoginModel(LoginViewModel login)
        {
            string password = login.NewPassword;
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
            return new Login()
            {
                EmployeeID = login.EmployeeID,
                ID = login.ID,
                Password = password,
                UserName = login.UserName
            };
        }
        private LoginViewModel GetLoginViewModel(Login login)
        {
            return new LoginViewModel()
            {
                EmployeeID = login.EmployeeID,
                ID = login.ID,
                FullName = login.Employee != null ? login.Employee.FullName : "",
                UserName = login.UserName,
            };
        }

        [HttpPost]

        [Authorization("Logins", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                int totalRecords = 0;

                var logins = _AccountRepo.GetAll();
                var result = logins.Select(n => GetLoginViewModel(n));
                Filtering<LoginViewModel> filtering = new Filtering<LoginViewModel>
                {
                    Columns = obj.FilteredColumns
                };

                string fullName = filtering.GetValue("FullName_");
                if (!string.IsNullOrEmpty(fullName))
                    result = result.Where(r => r.FullName.ToLower().Contains(fullName.ToLower()));

                string userName = filtering.GetValue("UserName_");
                if (!string.IsNullOrEmpty(userName))
                    result = result.Where(r => r.UserName.ToLower().Contains(userName.ToLower()));

                result = filtering.OrderBy(obj.OrderBy, result);

                totalRecords = result.Count();

                var data = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                int numberOfPages = (int)(Math.Ceiling(totalRecords * 1.0 / pageSize));

                return Json(new { NumberOfPages = numberOfPages, data });
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return Json(new { NumberOfPages = 0, data = string.Empty });
            }
        }
    }
}
