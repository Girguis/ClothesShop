using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Infrastructure;
using ClothesShop.Models;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class EmployeeBalanceController : Controller
    {
        private readonly EmployeeBalanceRepo employeeBalanceRepo;
        private readonly EmployeesRepo employeesRepo;

        public EmployeeBalanceController()
        {
            employeeBalanceRepo = new EmployeeBalanceRepo();
            employeesRepo = new EmployeesRepo();
        }
        private int GetJobID()
        {
            int jobId = 4;
            if (Session["JobID"] != null && !string.IsNullOrEmpty(Session["JobID"].ToString()))
                int.TryParse(Session["JobID"].ToString(), out jobId);
            return jobId;

        }
        private int GetUserID()
        {
            int empID = 0;
            if (Session["UserID"] != null && !string.IsNullOrEmpty(Session["UserID"].ToString()))
                int.TryParse(Session["UserID"].ToString(), out empID);
            return empID;
        }

        // GET: EmployeeBalance
        [Authorization("Balance", (RoleType.View))]

        public ActionResult Index() 
        {
            if (GetJobID() != (int)JobTypes.Manager)
                return RedirectToAction("Details", new { id = GetUserID() });
            return View();
        }
        [HttpPost]
        [Authorization("Balance", (RoleType.Edit))]
        public ActionResult GetAll()
        {
            var sellers = employeesRepo.GetAll(true)
                .Where(x => x.JobTypeID == (int)Enums.JobTypes.Worker)
                .Select(x=>new {x.ID,x.FullName });
            return Json(new { TotalCount = sellers.Count(), Data = sellers });
        }
        [HttpGet]
        [Authorization("Balance", (RoleType.Add))]
        public ActionResult Create(int sellerID)
        {
            var emp = employeesRepo.GetByID(sellerID);
            if (emp == null)
                return HttpNotFound();

            EmployeeBalanceViewModel model = new EmployeeBalanceViewModel
            {
                SellerID = sellerID,
                SellerName = emp.FullName
            };
            
            return View(model);
        }
        [HttpPost]
        [Authorization("Balance", (RoleType.Add))]
        public ActionResult Create(EmployeeBalanceViewModel model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    EmployeesBalance employeesBalance = new EmployeesBalance
                    {
                        Amount = model.Amount,
                        CreateDate = DateTime.Now,
                        EmployeeID = model.SellerID,
                        Type = model.Type
                    };
                    employeeBalanceRepo.Add(employeesBalance);
                    return RedirectToAction("Details",new { id = model.SellerID});
                }
                return View(model);
            }
            catch(Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return View(model);
            }
        }

        [HttpGet]
        [Authorization("Balance", (RoleType.Edit))]
        public ActionResult Edit(int id)
        {
            var res = employeeBalanceRepo.GetByID(id);
            if (res == null)
                return HttpNotFound();
            EmployeeBalanceViewModel model = new EmployeeBalanceViewModel
            { 
                ID= id,
                CreateDate = res.CreateDate.Value,
                Amount = res.Amount.Value,
                Type = res.Type.Value,
                SellerID = (int)res.EmployeeID,
                SellerName = res.Employee.FullName
            };
            return View(model);
        }
        [HttpPost]
        [Authorization("Balance", (RoleType.Edit))]
        public ActionResult Edit(EmployeeBalanceViewModel model)
        {
            if(ModelState.IsValid)
            {
                EmployeesBalance employeesBalance = new EmployeesBalance
                {
                    ID = model.ID,
                    Amount = model.Amount,
                    CreateDate = model.CreateDate,
                    EmployeeID = model.SellerID,
                    Type = model.Type
                };
                employeeBalanceRepo.Update(employeesBalance);
                return RedirectToAction("Details",new { id = model.SellerID});
            }
            return View(model);
        }

        [HttpGet]
        [Authorization("Balance", (RoleType.Details))]
        public ActionResult Details(int id)
        {
            if (GetJobID() != (int)JobTypes.Manager && GetUserID() != id)
                return RedirectToAction("Details", new { id=id});
            var emp = employeesRepo.GetByID(id);
            if (emp == null)
                return HttpNotFound();
            ViewBag.SellerName = emp.FullName;
            ViewBag.SellerID = id;
            return View();
        }
        [HttpPost]
        [Authorization("Balance", (RoleType.Details))]
        public ActionResult GetSellerBalance(int id)
        {
            var totalSellerBalances = employeeBalanceRepo.GetAll()
                                    .Where(x => x.EmployeeID == id)
                                    .Select(x=> new{ x.Amount, x.CreateDate.Value.Year,x.CreateDate.Value.Month,x.ID,x.Type});
            int totalRecords = totalSellerBalances.Count();
            return Json(new {TotalCount= totalRecords, Data=totalSellerBalances });
        }
        [HttpPost]
        [Authorization("Balance", (RoleType.Delete))]
        public JsonResult Delete(int? id)
        {
            if (!id.HasValue)
                return Json(false);
            return Json(employeeBalanceRepo.Delete(id.Value));
        }
    }
}