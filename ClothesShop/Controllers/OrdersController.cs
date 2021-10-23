using System;
using System.Collections.Generic;
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
using Authorization = ClothesShop.Infrastructure.Authorization;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class OrdersController : Controller
    {
        private readonly OrdersRepo _OrdersRepo;
        public OrdersController()
        {
            _OrdersRepo = new OrdersRepo();
        }
        [Authorization("Orders", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }

        [Authorization("Orders", (RoleType.Details))]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = _OrdersRepo.GetByID(id.Value);
            if (order == null)
            {
                return HttpNotFound();
            }
            var ex = GetOrdersViewModel(order);
            return View(ex);
        }
        [Authorization("Orders", (RoleType.Add))]
        public ActionResult Create()
        {
            OrdersViewModel model = new OrdersViewModel()
            {
                Product = new ProductsViewModel()
                {
                    ID = 0,
                    NumberOfPieces=1,
                    SellingPrice=285
                }
            };
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Orders", (RoleType.Add))]
        public ActionResult Create(OrdersViewModel order)
        {
            ModelState.Remove("Product.ID");
            ModelState.Remove("Product.Name");
            ModelState.Remove("Product.NumberOfPieces");
            ModelState.Remove("Product.OriginalPrice");
            ModelState.Remove("Product.SellingPrice");
            ModelState.Remove("Product.ColorID");
            ModelState.Remove("Product.SizeID");

            if (ModelState.IsValid)
            {
                Order today = GetOrderModel(order, true);
                _OrdersRepo.Add(today);
                return RedirectToAction("Index");
            }
            return View(order);
        }
        [Authorization("Orders", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = _OrdersRepo.GetByID(id.Value);

            if (order == null)
            {
                return HttpNotFound();
            }

            var model = GetOrdersViewModel(order);
            if (order.OrderStatusID == (int)OrderStatuses.TotallyDelivered)
                return View("Details", model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Orders", (RoleType.Edit))]
        public ActionResult Edit(OrdersViewModel order)
        {
            ModelState.Remove("Product.ID");
            ModelState.Remove("Product.ProductID");
            ModelState.Remove("Product.Name");
            ModelState.Remove("Product.NumberOfPieces");
            ModelState.Remove("Product.OriginalPrice");
            ModelState.Remove("Product.SellingPrice");
            ModelState.Remove("Product.ColorID");
            ModelState.Remove("Product.SizeID");


            if (ModelState.IsValid)
            {
                if (order.OrderStatusID == (int)OrderStatuses.New || order.OrderStatusID == (int)OrderStatuses.Delayed ||
                   order.OrderStatusID == (int)OrderStatuses.NotDelivered || order.OrderStatusID == (int)OrderStatuses.CanceledByAgent)
                    order.EmployeeID = null;

                var model = GetOrderModel(order);
                _OrdersRepo.Update(model);
                return RedirectToAction("Index");
            }
            return View(order);
        }
        [HttpPost]
        [Authorization("Orders", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (!id.HasValue)
                return Json(false);
            return Json(_OrdersRepo.Delete(id.Value));
        }

        private OrdersViewModel GetOrdersViewModel(Order t)
        {
            OrdersViewModel orderViewModel = new OrdersViewModel();
            try
            {
                orderViewModel = new OrdersViewModel()
                {
                    ID = t.ID,
                    CreatedBy = t.CreatedBy,
                    CreatedOn = t.CreatedOn.Value,
                    ShipmentPrice = t.ShipmentPrice,
                    PaidAmount = t.PaidAmount.HasValue ? t.PaidAmount.Value : 0,
                    RequestDate = t.RequestDate,
                    DeliveryDate = t.DeliveryDate,
                    CustomerID = t.CustomerID,
                    CityID = t.CityID.HasValue ? t.CityID.Value : 1,
                    EmployeeID = t.EmployeeID.HasValue ? t.EmployeeID : 0,
                    SellerID = t.SellerID.HasValue ? t.SellerID : 0,
                    Notes = t.Notes,
                    OrderStatusID = t.OrderStatusID.HasValue ? t.OrderStatusID.Value : (int)Enums.OrderStatuses.New,
                    ShipmentCompanyID = t.ShipmentCompanyID.HasValue ? t.ShipmentCompanyID : 0,
                    Customer = new CustomerViewModel()
                    {
                        Address = t.Customer.Address,
                        ID = t.Customer.ID,
                        MobileNumber1 = t.Customer.MobileNumber1,
                        MobileNumber2 = t.Customer.MobileNumber2,
                        Name = t.Customer.Name
                    },
                    Products = t.ProductOrders.Select(x => new ProductsViewModel()
                    {
                        ID = x.ID,
                        ProductID = x.ProductID.HasValue ? x.ProductID.Value : 0,
                        ColorID = x.ColorID.HasValue ? x.ColorID.Value : 0,
                        ColorName = x.Color != null ? x.Color.Color1 : "",
                        Name = x.Product.Name,
                        NumberOfPieces = x.Quantity,
                        OriginalPrice = x.OrginalPrice.HasValue ? x.OrginalPrice.Value : 0,
                        SellingPrice = x.SellingPrice.HasValue ? x.SellingPrice.Value : 0,
                        SizeID = x.SizeID.HasValue ? x.SizeID.Value : 0,
                        SizeName = x.Size != null ? x.Size.Size1 : "",
                    }).ToList(),
                    Product = new ProductsViewModel()
                    {
                        ID = 0,
                    },
                    OrderTotalPrice = t.ProductOrders.Sum(p => p.Quantity * p.SellingPrice).Value,
                    OrderStatusName = Helper.EnumToList<Enums.OrderStatuses>().Where(x => x.ID == t.OrderStatusID).First().Name,
                    EmployeeName = t.Employee != null ? t.Employee.FullName : "",
                    CityName = t.City != null ? t.City.Name : "",
                    ShipmentCompanyName = t.ShipmentCompany != null ? t.ShipmentCompany.Name : "",
                    SellerName = t.Seller != null ? t.Seller.FullName : "",
                };
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return orderViewModel;
        }
        private Order GetOrderModel(OrdersViewModel t, bool isNew = false)
        {
            Order order = new Order();
            try
            {
                long? nullVal = null;
                order = new Order()
                {
                    ID = t.ID,
                    CreatedBy = isNew ? Session["UserName"].ToString() : t.CreatedBy,
                    CreatedOn = isNew ? DateTime.Now : t.CreatedOn,
                    ShipmentPrice = t.ShipmentPrice,
                    PaidAmount = t.PaidAmount,
                    RequestDate = isNew ? DateTime.Now : t.RequestDate,
                    DeliveryDate = t.DeliveryDate,
                    CustomerID = t.CustomerID,
                    CityID = t.CityID,
                    EmployeeID = t.EmployeeID.HasValue && t.EmployeeID != 0 ? t.EmployeeID.Value : nullVal,
                    SellerID = t.SellerID.HasValue && t.SellerID != 0 ? t.SellerID.Value : nullVal,
                    Notes = t.Notes,
                    OrderStatusID = t.OrderStatusID,
                    ShipmentCompanyID = t.ShipmentCompanyID.HasValue && t.ShipmentCompanyID != 0 ? t.ShipmentCompanyID.Value : nullVal,
                    Customer = new Customer()
                    {
                        Address = t.Customer.Address,
                        ID = t.Customer.ID,
                        MobileNumber1 = t.Customer.MobileNumber1,
                        MobileNumber2 = t.Customer.MobileNumber2,
                        Name = t.Customer.Name
                    },
                    ProductOrders = t.Products.Select(x => new ProductOrder()
                    {
                        ID = x.ID,
                        ProductID = x.ProductID,
                        ColorID = x.ColorID,
                        Quantity = x.NumberOfPieces,
                        OrginalPrice = x.OriginalPrice,
                        SellingPrice = x.SellingPrice,
                        SizeID = x.SizeID,
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return order;
        }

        [HttpPost]
        [Authorization("Orders", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageNumber = obj.PageNumber > 0 ? obj.PageNumber : 1;
                var pageSize = obj.PageSize;
                int totalRecords = 0;

                Filtering<OrdersViewModel> filtering = new Filtering<OrdersViewModel>();
                filtering.Columns = obj.FilteredColumns;

                int? orderId = null;
                string requestDate = null;
                int? orderStatusId = null;
                if (filtering.GetValue("ID") != null && !string.IsNullOrEmpty(filtering.GetValue("ID")))
                    orderId = int.Parse(filtering.GetValue("ID"));

                if (filtering.GetValue("OrderStatusID") != null && !string.IsNullOrEmpty(filtering.GetValue("OrderStatusID")))
                    orderStatusId = int.Parse(filtering.GetValue("OrderStatusID"));

                if (filtering.GetValue("RequestDate") != null && !string.IsNullOrEmpty(filtering.GetValue("RequestDate")))
                    requestDate = DateTime.ParseExact(filtering.GetValue("RequestDate"), "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture).ToString("MM/dd/yyyy");

                string name = filtering.GetValue("Customer_Name");
                string mobileNumber1 = filtering.GetValue("Customer_MobileNumber1");
                string address = filtering.GetValue("Customer_Address");
                string deliveryName = filtering.GetValue("EmployeeName");
                string sellerName = filtering.GetValue("SellerName");
                string orderBy = obj.OrderBy.ColumnName;
                string orderDirection = obj.OrderBy.Direction;

                List<OrderViewModel> data = _OrdersRepo.Get(orderId, requestDate, name, mobileNumber1, orderStatusId, sellerName, deliveryName, orderBy, orderDirection, pageNumber, pageSize, out totalRecords).ToList();
                if (data != null && data.Count() > 0)
                    data = data.Select(c => { c.OrderStatusName = Helper.EnumToList<OrderStatuses>().Where(x => x.ID == c.OrderStatusID).First().Name; return c; }).ToList();
                //totalRecords = result.Count();
                int numberOfPages = (int)(Math.Ceiling(totalRecords * 1.0 / pageSize));
                return Json(new { NumberOfPages = numberOfPages, data = data });
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return Json(new { NumberOfPages = 0, data = string.Empty });
            }
        }

        [HttpPost]
        [Authorization("Orders", (RoleType.View))]
        public ActionResult GetAllUnAssignedOrders(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                int totalRecords = 0;
                var orderStatuses = Helper.EnumToList<Enums.OrderStatuses>();
                var orders = _OrdersRepo.GetAll().Where(o =>
                                                          (
                                                            o.OrderStatusID == (int)OrderStatuses.New ||
                                                            o.OrderStatusID == (int)OrderStatuses.Delayed ||
                                                            o.OrderStatusID == (int)OrderStatuses.NotDelivered
                                                           )
                                                           && !o.EmployeeID.HasValue);
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
                    OrderStatusName = orderStatuses.Where(x => x.ID == n.OrderStatusID).First().Name
                });
                Filtering<OrdersViewModel> filtering = new Filtering<OrdersViewModel>();
                filtering.Columns = obj.FilteredColumns;

                result = filtering.Search("ID", result);
                string name = filtering.GetValue("Customer.Name");
                if (!string.IsNullOrEmpty(name))
                    result = result.Where(c => c.Customer.Name != null && c.Customer.Name.ToLower().Contains(name.ToLower()));

                string address = filtering.GetValue("Customer.Address");
                if (!string.IsNullOrEmpty(address))
                    result = result.Where(c => c.Customer.Address != null && c.Customer.Address.ToLower().Contains(address.ToLower()));

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
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~\Reports\Invoice.rdlc");
                var x = _OrdersRepo.GetByID(id);
                var orderDataSet = new
                {
                    CityName = x.City != null ? x.City.Name : "",
                    CustomerAddress = x.Customer != null ? x.Customer.Address : "",
                    CustomerMobileNumber = x.Customer != null ? string.Join(" - ", new string[] { x.Customer.MobileNumber1, x.Customer.MobileNumber2 }) : "",
                    CustomerName = x.Customer != null ? x.Customer.Name : "",
                    Notes = x.Notes != null ? x.Notes : Languages.Resources.NoNotes,
                    OrderID = x.ID,
                    ShipmentPrice = x.ShipmentPrice.HasValue ? x.ShipmentPrice : 0,
                    DisplayDate = DateTime.Now,
                };

                var orderDetailsDataSet = x.ProductOrders.Select(v => new
                {
                    ProductName = "( " + (v.Size != null ? v.Size.Size1 : "") + " - " + (v.Color != null ? v.Color.Color1 : "") + " ) " + (v.Product != null ? v.Product.Name : ""),
                    Quantity = v.Quantity,
                    SellingPrice = v.SellingPrice
                }).ToList();
                reportViewer.LocalReport.DataSources.Clear();
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("OrderDataSet", new[] { orderDataSet }.ToList()));
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("OrderDetailsDataSet", orderDetailsDataSet));
                reportViewer.LocalReport.Refresh();


                //Byte  
                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, filenameExtension;

                byte[] bytes = reportViewer.LocalReport.Render("Pdf", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                //File  
                string FileName = "Order_" + DateTime.Now.Ticks.ToString() + ".pdf";
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
    }
}
