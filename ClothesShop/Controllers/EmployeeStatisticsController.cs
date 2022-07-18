using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Infrastructure;
using ClothesShop.Models;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class EmployeeStatisticsController : Controller
    {
        private readonly EmployeesRepo employeesRepo;
        private readonly OrdersRepo ordersRepo;
        private readonly EmployeeBalanceRepo balanceRepo;

        public EmployeeStatisticsController()
        {
            employeesRepo = new EmployeesRepo();
            ordersRepo = new OrdersRepo();
            balanceRepo = new EmployeeBalanceRepo();
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
        [Authorization("Balance", (RoleType.View))]
        public ActionResult Index()
        {
            if (GetJobID() != (int)JobTypes.Manager)
            {
                return RedirectToAction("SellerStatistics", "EmployeeStatistics", new {area="" });
            }
                
            return View();
        }
        // GET: EmployeeStatistics
        [HttpPost]
        [Authorization("Balance", (RoleType.View))]
        public ActionResult GetAll(DateTime? date)
        {
            if (GetJobID() != (int)JobTypes.Manager)
                return HttpNotFound();
            try
            {
                int year, month;
                if (!date.HasValue)
                {
                    year = DateTime.Now.Year;
                    month = DateTime.Now.Month;
                }
                else
                {
                    year = date.Value.Year;
                    month = date.Value.Month;
                }

                var allEmps = employeesRepo.GetAll(true);
                List<Employee> sellers = allEmps.Where(x => x.JobTypeID == (int)Enums.JobTypes.Worker).ToList();
                var data = new List<EmployeeStatisticsViewModel>();
                foreach (var seller in sellers)
                {
                    var empOrders = ordersRepo.GetAll()
                        .Where(x => x.SellerID == seller.ID)
                        .Where(y => (y.RequestDate.Value.Year == year)
                                && (y.RequestDate.Value.Month == month));

                    data.Add(new EmployeeStatisticsViewModel
                    {
                        SellerID = seller.ID,
                        SellerName = seller.FullName,
                        CanceledByAgent = empOrders.Where(x => x.OrderStatusID == (int)Enums.OrderStatuses.CanceledByAgent).Count(),
                        New = empOrders.Where(x => x.OrderStatusID == (int)Enums.OrderStatuses.New).Count(),
                        PartialyDelivered = empOrders.Where(x => x.OrderStatusID == (int)Enums.OrderStatuses.PartialyDelivered).Count(),
                        TotallyDelivered = empOrders.Where(x => x.OrderStatusID == (int)Enums.OrderStatuses.TotallyDelivered).Count(),
                    });
                }
                data.ForEach(x => x.Total = x.New + x.PartialyDelivered + x.TotallyDelivered + x.CanceledByAgent);

                int totalRecords = data.Count;
                return Json(new { TotalCount = totalRecords, Data = data });
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return Json(new { TotalCount = 0, Data = string.Empty });
            }
        }

        [HttpGet]
        [Authorization("Balance", (RoleType.View))]
        public ActionResult SellerStatistics()
        {
            var seller = employeesRepo.GetByID(GetUserID());
            if (seller == null)
                return HttpNotFound();
            SingleSellerStaticticsViewModel m = new SingleSellerStaticticsViewModel
            {
                SellerID = seller.ID,
            };
            return View(m);
        }
        [HttpGet]
        [Authorization("Balance", (RoleType.View))]
        public ActionResult GetSellerStatistics(int id, DateTime? date)
        {
            try
            {
                int year = date.HasValue ? date.Value.Year : DateTime.Now.Year;
                int month = date.HasValue ? date.Value.Month : DateTime.Now.Month;

                var seller = employeesRepo.GetByID(id);
                if (seller == null)
                    return HttpNotFound();
                var empOrders = ordersRepo.GetAll()
                     .Where(x => x.SellerID == seller.ID)
                     .Where(y => (y.RequestDate.Value.Year == year)
                             && (y.RequestDate.Value.Month == month));
                var empBalance = balanceRepo.GetAll()
                    .Where(x => x.EmployeeID == id &&
                    x.CreateDate.Value.Year == year &&
                    x.CreateDate.Value.Month == month);
                SingleSellerStaticticsViewModel model = new SingleSellerStaticticsViewModel
                {
                    SellerID = seller.ID,
                    SellerName = seller.FullName,
                    New = empOrders.Where(x => x.OrderStatusID == (int)OrderStatuses.New).Count(),
                    Waiting = empOrders.Where(x => x.OrderStatusID == (int)OrderStatuses.Waiting).Count(),
                    TotallyDelivered = empOrders.Where(x => x.OrderStatusID == (int)OrderStatuses.TotallyDelivered).Count(),
                    PartialyDelivered = empOrders.Where(x => x.OrderStatusID == (int)OrderStatuses.PartialyDelivered).Count(),
                    CanceledByAgent = empOrders.Where(x => x.OrderStatusID == (int)OrderStatuses.CanceledByAgent).Count(),
                    TotalBalance = empBalance.Where(x => x.Type == (int)BalanceType.Balance).Sum(x => x.Amount).Value,
                    TotalWithdraw = empBalance.Where(x => x.Type == (int)BalanceType.Withdraw).Sum(x => x.Amount).Value,
                    TotalDeduction = empBalance.Where(x => x.Type == (int)BalanceType.Deduction).Sum(x => x.Amount).Value,
                    TotalReward = empBalance.Where(x => x.Type == (int)BalanceType.Reward).Sum(x => x.Amount).Value
                };
                model.TotalOrders = model.New + model.Waiting + model.TotallyDelivered + model.PartialyDelivered + model.CanceledByAgent;
                return PartialView("_SellerStatistics", model);
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return View("InternalError", "Error");
            }

        }
    }
}