using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Helpers;
using ClothesShop.Infrastructure;
using ClothesShop.Models;
using DAL;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using Spire.Xls;
using Authorization = ClothesShop.Infrastructure.Authorization;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class DeliveryController : Controller
    {
        private readonly OrdersRepo _OrdersRepo;

        private readonly EmployeesRepo _EmployeesRepo;
        public DeliveryController()
        {
            _OrdersRepo = new OrdersRepo();
            _EmployeesRepo = new EmployeesRepo();
        }
        // GET: Delivery
        [Authorization("Delivery", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }

        [Authorization("Delivery", (RoleType.Add))]
        public ActionResult Create(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = _EmployeesRepo.GetByID(id.Value);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeName = employee.FullName;
            ViewBag.ID = employee.ID;
            ViewBag.EmployeeMobileNumber = employee.MobileNumber1;
            return View();
        }
        [HttpPost]
        [Authorization("Delivery", (RoleType.Add))]
        public ActionResult Create(long? employeeId, List<long> orderIds)
        {
            if (employeeId == null || orderIds == null || orderIds.Count <= 0)
            {
                return Json(false);
            }
            bool result = _OrdersRepo.AssignOrdersToDeliveryMan(employeeId.Value, orderIds);
            return Json(result);
        }

        // GET: Delivery/Edit/5
        [Authorization("Delivery", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = _EmployeesRepo.GetByID(id.Value);
            if (employee == null)
            {
                return HttpNotFound();
            }
            var model = GetOrderDeliveryViewModel(employee);

            var orderStatuses = Helper.EnumToList<OrderStatuses>();
            var orders = employee.Orders.Where(o => o.OrderStatusID == (int)OrderStatuses.Waiting);
            var result = orders.Select(n => new OrdersViewModel
            {
                ID = n.ID,
                RequestDate = n.RequestDate,
                Customer = new CustomerViewModel()
                {
                    Address = n.Customer != null ? n.Customer.Address : "",
                    Name = n.Customer != null ? n.Customer.Name : "",
                    MobileNumber1 = n.Customer != null ? n.Customer.MobileNumber1 : "",
                    MobileNumber2 = n.Customer != null ? n.Customer.MobileNumber2 : "",
                },
                OrderTotalPrice = n.ProductOrders.Sum(p => p.Quantity * p.SellingPrice).Value,
                ShipmentPrice = n.ShipmentPrice,
                PaidAmount = (n.PaidAmount.HasValue ? n.PaidAmount.Value : 0),
                OrderStatusName = orderStatuses.Where(x => x.ID == n.OrderStatusID).First().Name,
                OrderStatusID = n.OrderStatusID.HasValue ? n.OrderStatusID.Value : 0,
            }).ToList();

            ViewBag.EmployeeName = employee.FullName;
            ViewBag.ID = employee.ID;
            ViewBag.EmployeeMobileNumber = employee.MobileNumber1;
            ViewBag.Orders = result;

            return View();
        }

        // POST: Delivery/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Delivery", (RoleType.Edit))]
        public ActionResult Edit(OrderDeliveryViewModel order)
        {
            if (ModelState.IsValid)
            {
                var model = GetOrderModel(order);
                _OrdersRepo.Update(model);
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Delivery/Delete/5
        [HttpPost]
        [Authorization("Delivery", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (!id.HasValue)
                return Json(false);
            return Json(_OrdersRepo.Delete(id.Value));
        }
        [HttpPost]
        [Authorization("Delivery", (RoleType.Edit))]
        public JsonResult ChangeOrderStatus(long? orderId, long? deliverManId, OrderStatuses orderStatusId, double? paidAmount)
        {
            if (!orderId.HasValue)
                return Json(false);
            var order = _OrdersRepo.GetByID(orderId.Value);
            if (orderStatusId == OrderStatuses.New || orderStatusId == OrderStatuses.Delayed || orderStatusId == OrderStatuses.NotDelivered || orderStatusId == OrderStatuses.CanceledByAgent)
                deliverManId = null;

            order.EmployeeID = deliverManId;
            order.OrderStatusID = (int)orderStatusId;
            order.PaidAmount = paidAmount;
            return Json(_OrdersRepo.ChangeOrderStatus(order));
        }
        [HttpPost]
        [Authorization("Delivery", (RoleType.Edit))]
        public JsonResult ChangeOrdersStatus(List<long> orderIds, OrderStatuses orderStatusId)
        {
            if (orderIds == null || orderIds.Count <= 0)
                return Json(false);
            foreach (long orderId in orderIds)
            {
                var order = _OrdersRepo.GetByID(orderId);
                if (orderStatusId == OrderStatuses.New || orderStatusId == OrderStatuses.Delayed || orderStatusId == OrderStatuses.NotDelivered || orderStatusId == OrderStatuses.CanceledByAgent)
                    order.EmployeeID = null;
                order.OrderStatusID = (int)orderStatusId;
                if (!_OrdersRepo.ChangeOrderStatus(order))
                    return Json(false);
            }
            return Json(true);
        }

        private OrderDeliveryViewModel GetOrderDeliveryViewModel(Employee o)
        {
            var orders = o.Orders != null ? o.Orders.Where(c => c.OrderStatusID == (int)Enums.OrderStatuses.Waiting) : new List<Order>();
            return new OrderDeliveryViewModel()
            {
                EmployeeID = o.ID,
                EmployeeMobileNumber = o.MobileNumber1,
                EmployeeName = o.FullName,
                NumberOfOrders = orders.Count(),
                TotalOrderCash = orders.Sum(i => i.ProductOrders.Sum(oo => oo.Quantity * oo.SellingPrice)).Value
            };
        }
        private Order GetOrderModel(OrderDeliveryViewModel o)
        {
            return new Order()
            {
            };
        }

        [HttpPost]
        [Authorization("Delivery", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                int totalRecords = 0;

                var employees = _EmployeesRepo.GetAll(true).Where(o => o.JobTypeID == (int)Enums.JobTypes.DeliveryMan);
                var result = employees.Select(n => GetOrderDeliveryViewModel(n));
                Filtering<OrderDeliveryViewModel> filtering = new Filtering<OrderDeliveryViewModel>();
                filtering.Columns = obj.FilteredColumns;

                result = filtering.Search(result);

                //Sorting    
                result = filtering.OrderBy(obj.OrderBy, result);

                totalRecords = result.Count();
                int numberOfPages = (int)(Math.Ceiling(totalRecords * 1.0 / pageSize));
                var data = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { NumberOfPages = numberOfPages, data = data });
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return Json(new { NumberOfPages = 0, data = string.Empty });
            }
        }


        public ActionResult ExportToPDF(long id)
        {
            try
            {

                //Report  
                ReportViewer reportViewer = new ReportViewer();

                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~\Reports\Sheet.rdlc");
                var employee = _EmployeesRepo.GetByID(id);
                var orders = employee.Orders.Where(o => o.OrderStatusID == (int)OrderStatuses.Waiting);
                var deliveryDetailsDataSet = orders.Select(n => new
                {
                    CustomerName = n.Customer != null ? n.Customer.Name : "",
                    CustomerAddress = n.Customer != null ? GetLastPartOfAddress(n.Customer.Address) : "",
                    CustomerMobileNumber = n.Customer.MobileNumber1,
                    SellerName = n.Seller != null ? n.Seller.FullName : "",
                    OrderPrice = n.ProductOrders.Sum(p => p.Quantity * p.SellingPrice).Value,
                    ShipmentPrice = n.ShipmentPrice,
                    OrderCode = n.ID,
                    RequestDate = n.RequestDate.Value.ToString("dd/MM/yyyy")
                }).ToList();
                var deliveryDataSet = new
                {
                    CurrentDate = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
                    DeliveryFullName = employee.FullName,
                    DeliveryMobileNumber = string.Join(" - ", new string[] { employee.MobileNumber1, employee.MobileNumber2 }),
                    NumberOfOrders = orders.Count(),
                    TotalOrderPrice = orders.Sum(o => o.ProductOrders.Sum(x => x.Quantity * x.SellingPrice)),
                    TotalShipmentPrice = orders.Sum(o => o.ShipmentPrice)
                };
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DeliveryDataSet", new[] { deliveryDataSet }.ToList()));
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DeliveryDetailsDataSet", deliveryDetailsDataSet));
                reportViewer.LocalReport.Refresh();
                //Byte  
                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, filenameExtension;

                byte[] bytes = reportViewer.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                //File  
                string FileName = "Sheet_" + DateTime.Now.Ticks.ToString() + ".pdf";
                string dir = HttpContext.Server.MapPath(@"~\Images\TempFiles\");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                string FilePath = dir + FileName;

                //create and set PdfReader  
                PdfReader reader = new PdfReader(bytes);
                FileStream output = new FileStream(FilePath, FileMode.Create);

                string Agent = HttpContext.Request.Headers["User-Agent"].ToString();

                //create and set PdfStamper  
                PdfStamper pdfStamper = new PdfStamper(reader, output, '0', true);

                if (Agent.Contains("Firefox"))
                    pdfStamper.JavaScript = "var res = app.loaded('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);');";
                else
                    pdfStamper.JavaScript = "var res = app.setTimeOut('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);', 200);";

                pdfStamper.FormFlattening = false;
                pdfStamper.Close();
                reader.Close();

                //return file path  
                string FilePathReturn = @"Images/TempFiles/" + FileName;
                string app = "";
                if (!HttpContext.Request.Url.AbsoluteUri.ToLower().Contains("clothesshop.local"))
                    app = "ClothesShop/";

                return Content(app + FilePathReturn);
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }


        public ActionResult ExportToExcel(long id)
        {
            try
            {
                Workbook workbook = new Workbook();
                Worksheet sheet = workbook.Worksheets[0];
                List<string> cols = new List<string>() { "التاريخ", "اسم المندوب", "رقم المندوب", "عدد الطلبات", "اجمالى مبلغ الطلبات", "اجمالى مبلغ الشحن", "موبايل", "", "" };
                List<string> colNames = new List<string>() { "A", "B", "C", "D", "E", "F", "G", "H", "I" };

                var employee = _EmployeesRepo.GetByID(id);
                var orders = employee.Orders.Where(o => o.OrderStatusID == (int)OrderStatuses.Waiting);
                var deliveryDetailsData = orders.Select(n => new
                {
                    CustomerName = n.Customer != null ? n.Customer.Name : "",
                    CustomerAddress = n.Customer != null ? GetLastPartOfAddress(n.Customer.Address) : "",
                    CustomerMobileNumber = n.Customer.MobileNumber1.ToString(),
                    SellerName = n.Seller != null ? n.Seller.FullName : "",
                    OrderPrice = n.ProductOrders.Sum(p => p.Quantity * p.SellingPrice).Value.ToString(),
                    ShipmentPrice = n.ShipmentPrice.ToString(),
                    OrderCode = n.ID.ToString(),
                    RequestDate = n.RequestDate.Value.ToString("yyyy-MM-dd")
                }).ToList();

                List<string> deliveryData = new List<string>()
                {
                    DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"),
                    employee.FullName,
                    string.Join(" - ", new string[] { employee.MobileNumber1, employee.MobileNumber2 }),
                    orders.Count().ToString(),
                    orders.Sum(o => o.ProductOrders.Sum(x => x.Quantity * x.SellingPrice)).ToString(),
                    orders.Sum(o => o.ShipmentPrice).ToString(),
                    ConfigurationManager.AppSettings["phoneNumbers"],"",""
                };
                var count = 1;
                sheet.Range["A1:G1"].Style.Color = System.Drawing.Color.FromArgb(150, 212, 255);
                sheet.Range["A1:G1"].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["A1:G1"].Style.Font.IsBold = true;
                sheet.Range["A1:G1"].BorderAround(LineStyleType.Thin, System.Drawing.Color.FromArgb(221, 221, 221));
                sheet.Range["A1:G1"].BorderInside(LineStyleType.Thin, System.Drawing.Color.FromArgb(221, 221, 221));
                for (int i = 0; i < cols.Count; i++)
                {
                    sheet.Range[colNames[i] + count].Value = cols[i];
                    sheet.Range[colNames[i] + (count + 1)].Value = deliveryData[i];
                }
                count += 3;
                List<string> cols2 = new List<string>() { "ع", "كود الطلب", "اسم العميل", "رقم العميل", "العنوان", "المبلغ", "حالة الطلب", "اسم البائع", "تاريخ الطلب" };
                sheet.Range["A4:I4"].Style.Color = System.Drawing.Color.FromArgb(150, 212, 255);
                sheet.Range["A4:I4"].Style.HorizontalAlignment = HorizontalAlignType.Center;
                sheet.Range["A4:I4"].Style.Font.IsBold = true;
                sheet.Range["A4:I4"].BorderAround(LineStyleType.Thin, System.Drawing.Color.FromArgb(221, 221, 221));
                sheet.Range["A4:I4"].BorderInside(LineStyleType.Thin, System.Drawing.Color.FromArgb(221, 221, 221));

                for (int i = 0; i < cols2.Count; i++)
                {
                    sheet.Range[colNames[i] + count].Value = cols2[i];
                }
                count++;
                for (int i = 0; i < deliveryDetailsData.Count; i++)
                {

                    sheet.Range[colNames[0] + count].Value = (i + 1).ToString();
                    sheet.Range[colNames[1] + count].Value = deliveryDetailsData[i].OrderCode;
                    sheet.Range[colNames[2] + count].Value = deliveryDetailsData[i].CustomerName;
                    sheet.Range[colNames[3] + count].Value = deliveryDetailsData[i].CustomerMobileNumber;
                    sheet.Range[colNames[4] + count].Value = deliveryDetailsData[i].CustomerAddress;
                    sheet.Range[colNames[5] + count].Value = deliveryDetailsData[i].OrderPrice;
                    sheet.Range[colNames[7] + count].Value = deliveryDetailsData[i].SellerName;
                    sheet.Range[colNames[8] + count].Value = deliveryDetailsData[i].RequestDate;
                    count++;
                }
                sheet.Range.Style.Font.Size = 14;
                sheet.AllocatedRange.AutoFitColumns();
                sheet.AllocatedRange.AutoFitRows();

                string FileName = "Sheet_" + DateTime.Now.Ticks.ToString() + ".xlsx";
                string dir = HttpContext.Server.MapPath(@"~\Images\ExcelFiles\");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                string strFilePath = dir + FileName;
                workbook.SaveToFile(strFilePath);

                string FilePathReturn = @"Images/ExcelFiles/" + FileName;
                string app = "";
                if (!HttpContext.Request.Url.AbsoluteUri.ToLower().Contains("clothesshop.local"))
                    app = "ClothesShop/";

                return Content(FilePathReturn);
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return null;
            }
        }
        private string GetLastPartOfAddress(string address)
        {
            string newAddress = "";
            try
            {
                if (string.IsNullOrEmpty(address))
                    return newAddress;
                var arr = address.Split('-');
                if (arr.Length <= 1)
                    newAddress = address;
                else
                    newAddress = arr[arr.Length - 1];
                return newAddress;
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return newAddress;
        }
    }
}
