﻿using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Helpers;
using ClothesShop.Infrastructure;
using ClothesShop.Models;
using DAL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Authorization = ClothesShop.Infrastructure.Authorization;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class OrdersController : BaseController
    {
        private readonly OrdersRepo _OrdersRepo;
        private static Dictionary<string,RoleType> _roles = new Dictionary<string, RoleType>() { { "Orders1", RoleType.Details }, { "Orders2", RoleType.Details } };
        public OrdersController()
        {
            _OrdersRepo = new OrdersRepo();
        }
        [Authorization("Orders1,Orders2", (RoleType.View))]
        public ActionResult Index(string PageID)
        {
            ViewBag.PageID = PageID;
            if (hasPageOrderAccess(PageID,RoleType.View))
                return View();
            else
                return View("Error");
        }
        private bool hasPageOrderAccess(string pageID,RoleType roleType)
        {
            //Page1->137D0514-F286-48AA-BCD4-B7FE7C5B79D8
            //Page2->80DB4628-F4D2-45EA-B82F-B0E2B7E9FD09
            return ((pageID == "137D0514-F286-48AA-BCD4-B7FE7C5B79D8" && RolesHelper.CheckRoleRight("Orders1", roleType))
                    || (pageID == "80DB4628-F4D2-45EA-B82F-B0E2B7E9FD09" && RolesHelper.CheckRoleRight("Orders2", roleType)));
        }
        private bool CheckEmpOrder(long? empSellerId, long? deliveryId)
        {
            int empId = GetUserID();
            if (GetJobID() != (int)JobTypes.DeliveryMan)
                return true;
            return empId == deliveryId;
        }

        [Authorization("Orders1,Orders2", (RoleType.View))]
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
            if (CheckEmpOrder(ex.SellerID, ex.EmployeeID) && hasPageOrderAccess(ex.PageID,RoleType.Details))
                return View(ex);
            else
                return RedirectToAction("Index", "Orders",new {PageID =ex.PageID });
        }
        [Authorization("Orders1,Orders2", (RoleType.Add))]
        public ActionResult Create(string pageID)
        {
            OrdersViewModel model = new OrdersViewModel();
            model.PageID = pageID;
            if (GetJobID() != (int)JobTypes.Manager)
                model.SellerID = GetUserID();
            model.OrderStatusID = (int)OrderStatuses.New;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Orders1,Orders2", (RoleType.Add))]
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
                if (GetJobID() != (int)JobTypes.Manager && today.SellerID == null)
                    today.SellerID = GetUserID();
                _OrdersRepo.Add(today);
                return RedirectToAction("Index", new { PageID = order.PageID});
            }
            return View(order);
        }
        [Authorization("Orders1,Orders2", (RoleType.Edit))]
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
            if (CheckEmpOrder(model.SellerID, model.EmployeeID) && hasPageOrderAccess(model.PageID,RoleType.Edit))
            {
                if (order.OrderStatusID == (int)OrderStatuses.TotallyDelivered)
                    return View("Details", model);
                else
                    return View(model);
            }
            else
                return RedirectToAction("Index", "Orders",new { PageID = model.PageID});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Orders1,Orders2", (RoleType.Edit))]
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
                return RedirectToAction("Index",new { PageID = model.PageID});
            }
            return View(order);
        }
        [HttpPost]
        [Authorization("Orders1,Orders2", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (!id.HasValue)
                return Json(false);
            var order = _OrdersRepo.GetByID(id.Value);
            if (!CheckEmpOrder(order.SellerID, order.EmployeeID))
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
                    CreatedOn = t.CreatedOn.Value.AddHours(GetUtcOffset()),
                    ShipmentPrice = t.ShipmentPrice,
                    PaidAmount = t.PaidAmount.HasValue ? t.PaidAmount.Value : 0,
                    RequestDate = t.RequestDate.Value.AddHours(GetUtcOffset()),
                    DeliveryDate = t.DeliveryDate,
                    CustomerID = t.CustomerID,
                    CityID = t.CityID.HasValue ? t.CityID.Value : 1,
                    EmployeeID = t.EmployeeID.HasValue ? t.EmployeeID : 0,
                    SellerID = t.SellerID.HasValue ? t.SellerID : 0,
                    Notes = t.Notes,
                    PageID = t.PageID,
                    OrderStatusID = t.OrderStatusID.HasValue ? t.OrderStatusID.Value : (int)OrderStatuses.New,
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
                orderViewModel.ProductSeralized = JsonConvert.SerializeObject(orderViewModel.Products);
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
                t.Products = JsonConvert.DeserializeObject<List<ProductsViewModel>>(t.ProductSeralized)?.ToList();
                order = new Order()
                {
                    ID = t.ID,
                    CreatedBy = isNew ? Session["UserName"].ToString() : t.CreatedBy,
                    CreatedOn = isNew ? DateTime.UtcNow : t.CreatedOn,
                    ShipmentPrice = t.ShipmentPrice,
                    PaidAmount = t.PaidAmount,
                    RequestDate = isNew ? DateTime.UtcNow : t.RequestDate,
                    DeliveryDate = t.DeliveryDate,
                    CustomerID = t.CustomerID,
                    CityID = t.CityID,
                    EmployeeID = t.EmployeeID.HasValue && t.EmployeeID != 0 ? t.EmployeeID.Value : nullVal,
                    SellerID = t.SellerID.HasValue && t.SellerID != 0 ? t.SellerID.Value : nullVal,
                    Notes = t.Notes?.Replace("\r\n", " ").Trim(),
                    OrderStatusID = t.OrderStatusID,
                    PageID = t.PageID,
                    ShipmentCompanyID = t.ShipmentCompanyID.HasValue && t.ShipmentCompanyID != 0 ? t.ShipmentCompanyID.Value : nullVal,
                    Customer = new Customer()
                    {
                        Address = t.Customer.Address.Replace("\r\n", " ").Trim(),
                        ID = t.Customer.ID,
                        MobileNumber1 = t.Customer.MobileNumber1,
                        MobileNumber2 = t.Customer.MobileNumber2,
                        Name = t.Customer.Name.Replace("\r\n", " ").Trim()
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
        [Authorization("Orders1,Orders2", (RoleType.View))]
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
                var result = orders.Select(n =>
                {
                    var sum = n.ProductOrders?.Sum(p => p.Quantity * p.SellingPrice);
                    return new OrdersViewModel
                    {

                        ID = n.ID,
                        RequestDate = n.RequestDate.Value.AddHours(GetUtcOffset()),
                        Customer = new CustomerViewModel()
                        {
                            Address = n.Customer != null ? n.Customer.Address : "",
                            Name = n.Customer != null ? n.Customer.Name : "",
                            MobileNumber1 = n.Customer != null ? n.Customer.MobileNumber1 : "",
                            MobileNumber2 = n.Customer != null ? n.Customer.MobileNumber2 : "",
                        },
                        OrderTotalPrice = sum ?? 0,
                        ShipmentPrice = n.ShipmentPrice,
                        OrderStatusName = orderStatuses?.Where(x => x.ID == n.OrderStatusID).FirstOrDefault()?.Name,
                        PageID = n.PageID,
                        SellerName = n.Seller?.FullName
                    };
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

                string seller = filtering.GetValue("SellerName");
                if (!string.IsNullOrEmpty(seller))
                    result = result.Where(c => c.SellerName != null && c.SellerName.ToLower().Contains(seller.ToLower()));


                result = filtering.OrderBy(obj.OrderBy, result);

                totalRecords = result.Count();
                var data = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { TotalCount = totalRecords, Data = data });
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return Json(new { TotalCount = 0, Data = string.Empty });
            }
        }


        [HttpPost]
        [Authorization("Orders1,Orders2", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj,string pageID)
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


                if (filtering.GetValue("RequestDate_") != null && !string.IsNullOrEmpty(filtering.GetValue("RequestDate_")))
                    requestDate = DateTime.ParseExact(filtering.GetValue("RequestDate_").Split(new string[] { " " }, StringSplitOptions.None)[0], "yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture).ToString("MM/dd/yyyy");

                string name = filtering.GetValue("Customer_Name");
                string mobileNumber1 = filtering.GetValue("Customer_MobileNumber1");
                string address = filtering.GetValue("Customer_Address");
                string deliveryName = filtering.GetValue("EmployeeName");
                string sellerName = filtering.GetValue("SellerName");
                string orderBy = obj.OrderBy?.ColumnName;
                if (obj.OrderBy?.ColumnName == "RequestDate_")
                    orderBy = "RequestDate";
                string orderDirection = obj.OrderBy?.Direction;
                List<OrderViewModel> data = null;
                if (GetJobID() != (int)JobTypes.DeliveryMan)
                    data = _OrdersRepo.Get(orderId, requestDate, name,address, mobileNumber1, orderStatusId, sellerName, deliveryName, orderBy, orderDirection, pageNumber, pageSize, null, pageID,out totalRecords).ToList();
                else
                    data = _OrdersRepo.Get(orderId, requestDate, name, address, mobileNumber1, orderStatusId, sellerName, deliveryName, orderBy, orderDirection, pageNumber, pageSize, GetUserID(),pageID ,out totalRecords).ToList();
                if (data != null && data.Count() > 0)
                    data = data.Select(c =>
                    {
                        var date = c.RequestDate.AddHours(GetUtcOffset());
                        c.OrderStatusName = Helper.EnumToList<OrderStatuses>()
                        .Where(x => x.ID == c.OrderStatusID).First().Name;
                        c.RequestDate = date;

                        c.RequestDate_ = date.ToString("dd/MM/yyyy hh:mm tt");
                        return c;
                    }).ToList();

                return Json(new { TotalCount = totalRecords, Data = data });
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return Json(new { TotalCount = 0, Data = string.Empty });
            }
        }

        [Authorization("Orders1,Orders2", (RoleType.Details))]
        public ActionResult ExportMultipleBills(string ids,string PageID)
        {
            try
            {
                List<int> orderIDs = ids.Split(new String[] { "," }, StringSplitOptions.None).Select(x => int.Parse(x)).ToList();
                string SourcePdfPath = HttpContext.Server.MapPath("~/Images/SourcePdfFiles/");
                string DestinationPdfPath = HttpContext.Server.MapPath("~/Images/DestPdfFile/");

                if (Directory.Exists(SourcePdfPath))
                {
                    DirectoryInfo di = new DirectoryInfo(SourcePdfPath);
                    var files = di.GetFiles();
                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                    }
                }
                else
                    Directory.CreateDirectory(SourcePdfPath);

                if (Directory.Exists(DestinationPdfPath))
                {
                    DirectoryInfo di = new DirectoryInfo(DestinationPdfPath);
                    var files = di.GetFiles();
                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                    }
                }
                else
                    Directory.CreateDirectory(DestinationPdfPath);

                foreach (var orderID in orderIDs)
                {
                    CreatePDF(orderID,PageID);
                }
                string[] filenames = Directory.GetFiles(SourcePdfPath);
                string outputFileName = "Orders_" + DateTime.Now.ToString("MMddyyyyhhmmss") + ".pdf";
                string outputPath = HttpContext.Server.MapPath("~/Images/DestPdfFile/" + outputFileName);

                Document doc = new Document();
                PdfCopy writer = new PdfCopy(doc, new FileStream(outputPath, FileMode.Create));
                if (writer == null)
                {
                    return null;
                }
                doc.Open();
                foreach (string filename in filenames)
                {
                    PdfReader reader = new PdfReader(filename);
                    reader.ConsolidateNamedDestinations();
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        writer.AddPage(page);
                    }
                    reader.Close();
                }
                writer.Close();
                doc.Close();

                var bytes = System.IO.File.ReadAllBytes(outputPath);
                PdfReader pdfReader = new PdfReader(bytes);
                FileStream output = new FileStream(outputPath, FileMode.Open);

                string Agent = HttpContext.Request.Headers["User-Agent"].ToString();

                //create and set PdfStamper  
                PdfStamper pdfStamper = new PdfStamper(pdfReader, output, '0', true);

                if (Agent.Contains("Firefox"))
                    pdfStamper.JavaScript = "var res = app.loaded('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);');";
                else
                    pdfStamper.JavaScript = "var res = app.setTimeOut('var pp = this.getPrintParams();pp.interactive = pp.constants.interactionLevel.full;this.print(pp);', 200);";

                pdfStamper.FormFlattening = false;
                pdfStamper.Close();
                pdfReader.Close();

                string app = ConfigurationManager.AppSettings["preExists"];

                string FilePathReturn = @"Images/DestPdfFile/" + outputFileName;
                return Content(app + FilePathReturn);
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }

        [Authorization("Orders1,Orders2", (RoleType.Details))]
        public String CreatePDF(long id, string PageID)
        {
            try
            {
                //Report  
                ReportViewer reportViewer = new ReportViewer();

                reportViewer.ProcessingMode = ProcessingMode.Local;
                if(PageID == "137D0514-F286-48AA-BCD4-B7FE7C5B79D8")
                    reportViewer.LocalReport.ReportPath = Server.MapPath(@"~\Reports\Invoice1.rdlc");
                else
                    reportViewer.LocalReport.ReportPath = Server.MapPath(@"~\Reports\Invoice2.rdlc");
                var x = _OrdersRepo.GetByID(id);
                var orderDataSet = new
                {
                    CityName = x.City != null ? x.City.Name : "",
                    CustomerAddress = x.Customer != null ? x.Customer.Address : "",
                    CustomerMobileNumber = x.Customer != null
                    ? (x.Customer.MobileNumber2 != null ? string.Join(" - ", new string[] { x.Customer.MobileNumber1, x.Customer.MobileNumber2 }) : x.Customer.MobileNumber1) : "",
                    CustomerName = x.Customer != null ? x.Customer.Name : "",
                    Notes = x.Notes != null ? x.Notes : Languages.Resources.NoNotes,
                    OrderID = x.ID,
                    ShipmentPrice = x.ShipmentPrice.HasValue ? x.ShipmentPrice : 0,
                    DisplayDate = DateTime.UtcNow.AddHours(GetUtcOffset()),
                    SellerName = x.Seller?.FullName
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
                string dir = HttpContext.Server.MapPath(@"~\Images\SourcePdfFiles\");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                string FilePath = dir + FileName;

                //create and set PdfReader  
                PdfReader reader = new PdfReader(bytes);
                FileStream output = new FileStream(FilePath, FileMode.Create);

                string Agent = HttpContext.Request.Headers["User-Agent"].ToString();

                //create and set PdfStamper  
                PdfStamper pdfStamper = new PdfStamper(reader, output, '0', true);


                pdfStamper.FormFlattening = false;
                pdfStamper.Close();
                reader.Close();

                return dir + FileName;
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }

        [Authorization("Orders1,Orders2", (RoleType.Details))]
        public ActionResult ExportToPDF(long id, string PageID)
        {
            try
            {
                //Report  
                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                if (PageID == "137D0514-F286-48AA-BCD4-B7FE7C5B79D8")
                    reportViewer.LocalReport.ReportPath = Server.MapPath(@"~\Reports\Invoice1.rdlc");
                else
                    reportViewer.LocalReport.ReportPath = Server.MapPath(@"~\Reports\Invoice2.rdlc");
                var x = _OrdersRepo.GetByID(id);
                var orderDataSet = new
                {
                    CityName = x.City != null ? x.City.Name : "",
                    CustomerAddress = x.Customer != null ? x.Customer.Address : "",
                    CustomerMobileNumber = x.Customer != null
                    ? (x.Customer.MobileNumber2 != null ? string.Join(" - ", new string[] { x.Customer.MobileNumber1, x.Customer.MobileNumber2 }) : x.Customer.MobileNumber1) : "",
                    CustomerName = x.Customer != null ? x.Customer.Name : "",
                    Notes = x.Notes != null ? x.Notes : Languages.Resources.NoNotes,
                    OrderID = x.ID,
                    ShipmentPrice = x.ShipmentPrice.HasValue ? x.ShipmentPrice : 0,
                    DisplayDate = DateTime.UtcNow.AddHours(GetUtcOffset()),
                    SellerName = x.Seller?.FullName
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
                if (Directory.Exists(dir))
                {
                    DirectoryInfo di = new DirectoryInfo(dir);
                    var files = di.GetFiles();
                    foreach (FileInfo file in files)
                    {
                        file.Delete();
                    }
                }
                else
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

                string app = ConfigurationManager.AppSettings["preExists"];

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
