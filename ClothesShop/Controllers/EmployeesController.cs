using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Infrastructure;
using ClothesShop.Models;
using DAL;
using Authorization = ClothesShop.Infrastructure.Authorization;

namespace ClothesShop.Controllers
{
    [Authentication]
    public class EmployeesController : Controller
    {
        private readonly EmployeesRepo _EmployeesRepo;
        public EmployeesController()
        {
            _EmployeesRepo = new EmployeesRepo();
        }
        // GET: Employees
        [Authorization("Employees", (RoleType.View))]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Employees/Details/5
        [Authorization("Employees", (RoleType.Details))]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = _EmployeesRepo.GetByID(id.Value);
            EmployeeViewModel employeeViewModel = GetEmployeeViewModel(employee);
            if (employeeViewModel == null)
            {
                return HttpNotFound();
            }

            ViewBag.FrontSSN = ConvertImageToBase64(employee.FrontSSNURL);
            ViewBag.BackSSN = ConvertImageToBase64(employee.BackSSNURL);

            ViewBag.FrontLicence = ConvertImageToBase64(employee.FrontLicenceURL);
            ViewBag.BackLicence = ConvertImageToBase64(employee.BackLicenceURL);

            return View(employeeViewModel);
        }

        // GET: Employees/Create
        [Authorization("Employees", (RoleType.Add))]
        public ActionResult Create()
        {
            var model = new EmployeeViewModel();
            return View(model);
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Employees", (RoleType.Add))]
        public ActionResult Create(EmployeeViewModel employeeViewModel)
        {
            if (employeeViewModel.FrontSSN != null)
                ModelState.Remove("FrontSSN");
            else
                ModelState.AddModelError("FrontSSN", Languages.Resources.RequiredField);

            if (employeeViewModel.BackSSN != null)
                ModelState.Remove("BackSSN");
            else
                ModelState.AddModelError("BackSSN", Languages.Resources.RequiredField);

            if ((employeeViewModel.FrontLicence == null || employeeViewModel.BackLicence == null)
                && employeeViewModel.JobTypeID == (int)Enums.JobTypes.DeliveryMan)
            {
                return View(employeeViewModel);
            }
            else
            {
                ModelState.Remove("FrontLicence");
                ModelState.Remove("BackLicence");
            }
            if (ModelState.IsValid)
            {
                var employee = GetEmployeeModel(employeeViewModel);
                _EmployeesRepo.Add(employee);

                employee.FrontSSNURL = UploadFile(employee.ID, employeeViewModel.FrontSSN);
                employee.BackSSNURL = UploadFile(employee.ID, employeeViewModel.BackSSN);
                employee.FrontLicenceURL = UploadFile(employee.ID, employeeViewModel.FrontLicence);
                employee.BackLicenceURL = UploadFile(employee.ID, employeeViewModel.BackLicence);
                _EmployeesRepo.Update(employee);

                return RedirectToAction("Index");
            }

            return View(employeeViewModel);
        }

        // GET: Employees/Edit/5
        [Authorization("Employees", (RoleType.Edit))]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = _EmployeesRepo.GetByID(id.Value);
            if (employee == null)
                return HttpNotFound();

            ViewBag.FrontSSN = ConvertImageToBase64(employee.FrontSSNURL);
            ViewBag.BackSSN = ConvertImageToBase64(employee.BackSSNURL);

            ViewBag.FrontLicence = ConvertImageToBase64(employee.FrontLicenceURL);
            ViewBag.BackLicence = ConvertImageToBase64(employee.BackLicenceURL);

            var employeeViewModel = GetEmployeeViewModel(employee, false);

            return View(employeeViewModel);
        }

        private dynamic ConvertImageToBase64(string path)
        {
            path = GetPhysicalPath(path);
            if (!System.IO.File.Exists(path))
                return "";
            else
                return "data:image;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes(path));
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization("Employees", (RoleType.Edit))]
        public ActionResult Edit(EmployeeViewModel employeeViewModel)
        {
            string fssnUrl = employeeViewModel.FrontSSNURL;
            string bssnUrl = employeeViewModel.BackSSNURL;

            if (employeeViewModel.FrontSSN != null)
            {
                if (!string.IsNullOrEmpty(fssnUrl) && System.IO.File.Exists(fssnUrl))
                    System.IO.File.Delete(fssnUrl);
                fssnUrl = UploadFile(employeeViewModel.ID, employeeViewModel.FrontSSN);
                ModelState.Remove("FrontSSN");
            }
            else if (string.IsNullOrEmpty(fssnUrl))
                ModelState.AddModelError("FrontSSN", Languages.Resources.RequiredField);
            else
                ModelState.Remove("FrontSSN");

            if (employeeViewModel.BackSSN != null)
            {
                if (!string.IsNullOrEmpty(bssnUrl) && System.IO.File.Exists(bssnUrl))
                    System.IO.File.Delete(bssnUrl);
                bssnUrl = UploadFile(employeeViewModel.ID, employeeViewModel.BackSSN);
                ModelState.Remove("BackSSN");
            }
            else if (string.IsNullOrEmpty(bssnUrl))
                ModelState.AddModelError("BackSSN", Languages.Resources.RequiredField);
            else
                ModelState.Remove("BackSSN");

            string fLicence = employeeViewModel.FrontLicenceURL;
            string bLicence = employeeViewModel.BackLicenceURL;

            if (employeeViewModel.JobTypeID == (int)Enums.JobTypes.DeliveryMan)
            {
                if (employeeViewModel.FrontLicence != null)
                {
                    if (!string.IsNullOrEmpty(fLicence) && System.IO.File.Exists(fLicence))
                        System.IO.File.Delete(fLicence);
                    fLicence = UploadFile(employeeViewModel.ID, employeeViewModel.FrontLicence);
                    ModelState.Remove("FrontLicence");
                }
                else if (string.IsNullOrEmpty(fLicence))
                    ModelState.AddModelError("FrontLicence", Languages.Resources.RequiredField);
                else
                    ModelState.Remove("FrontLicence");

                if (employeeViewModel.BackLicence != null)
                {
                    if (!string.IsNullOrEmpty(bLicence) && System.IO.File.Exists(bLicence))
                        System.IO.File.Delete(bLicence);
                    bLicence = UploadFile(employeeViewModel.ID, employeeViewModel.BackLicence);
                    ModelState.Remove("BackLicence");
                }
                else if (string.IsNullOrEmpty(bLicence))
                    ModelState.AddModelError("BackLicence", Languages.Resources.RequiredField);
                else
                    ModelState.Remove("BackLicence");
            }
            else
            {
                ModelState.Remove("FrontLicence");
                ModelState.Remove("BackLicence");
                employeeViewModel.FrontLicenceURL = "";
                employeeViewModel.BackLicenceURL = "";
            }
            if (ModelState.IsValid)
            {
                var employee = GetEmployeeModel(employeeViewModel);
                employee.FrontLicenceURL = fLicence;
                employee.BackLicenceURL = bLicence;
                employee.FrontSSNURL = fssnUrl;
                employee.BackSSNURL = bssnUrl;
                _EmployeesRepo.Update(employee);
                return RedirectToAction("Index");
            }
            return View(employeeViewModel);
        }

        // GET: Employees/Delete/5
        [HttpPost]
        [Authorization("Employees", (RoleType.Delete))]
        public JsonResult Delete(long? id)
        {
            if (id == null)
            {
                return Json(false);
            }
            bool isDeleted = _EmployeesRepo.Delete(id.Value);
            return Json(isDeleted);
        }

        private EmployeeViewModel GetEmployeeViewModel(Employee e, bool getURL = true)
        {
            return new EmployeeViewModel()
            {
                AdditionalInfo = e.AdditionalInfo,
                Address = e.Address,
                BackLicenceURL = getURL ? GetPhysicalPath(e.BackLicenceURL) : e.BackLicenceURL,
                BackSSNURL = getURL ? GetPhysicalPath(e.BackSSNURL) : e.BackSSNURL,
                BirthDate = e.BirthDate,
                CityID = e.CityID,
                FrontLicenceURL = getURL ? GetPhysicalPath(e.FrontLicenceURL) : e.FrontLicenceURL,
                FrontSSNURL = getURL ? GetPhysicalPath(e.FrontSSNURL) : e.FrontSSNURL,
                FullName = e.FullName,
                GenderID = e.GenderID,
                ID = e.ID,
                IsActive = e.IsActive,
                JobName = e.JobName,
                JobTypeID = e.JobTypeID,
                MobileNumber1 = e.MobileNumber1,
                MobileNumber2 = e.MobileNumber2,
                Salary = e.Salary,
                SSN = e.SSN,
                StartWorkingDate = e.StartWorkingDate
            };
        }

        private Employee GetEmployeeModel(EmployeeViewModel e)
        {
            return new Employee()
            {
                AdditionalInfo = e.AdditionalInfo,
                Address = e.Address,
                BackLicenceURL = e.BackLicenceURL,
                BackSSNURL = e.BackSSNURL,
                BirthDate = e.BirthDate,
                CityID = e.CityID,
                FrontLicenceURL = e.FrontLicenceURL,
                FrontSSNURL = e.FrontSSNURL,
                FullName = e.FullName,
                GenderID = e.GenderID,
                ID = e.ID,
                IsActive = e.IsActive,
                JobName = e.JobName,
                JobTypeID = e.JobTypeID.Value,
                MobileNumber1 = e.MobileNumber1,
                MobileNumber2 = e.MobileNumber2,
                Salary = e.Salary,
                SSN = e.SSN,
                StartWorkingDate = e.StartWorkingDate
            };
        }
        private string GetPhysicalPath(string path)
        {
            string dir = ConfigurationManager.AppSettings["ProjectPath"].ToString();
            if (string.IsNullOrEmpty(path))
                return "";
            else
                return dir + "\\" + path;
        }
        private string UploadFile(long employeeId, HttpPostedFileBase file)
        {
            try
            {
                if (file == null)
                    return "";

                string directory = ConfigurationManager.AppSettings["UploadedFilesDirectory"];
                string path = GetPhysicalPath(directory);
                if (string.IsNullOrEmpty(directory))
                    directory = "UploadedFiles";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path += "\\" + employeeId;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path += "\\" + file.FileName;
                file.SaveAs(path);
                return "UploadedFiles" + "\\" + employeeId + "\\" + file.FileName;
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }

        [HttpPost]

        [Authorization("Employees", (RoleType.View))]
        public ActionResult GetAll(SearchViewModel obj)
        {
            try
            {
                var pageIndex = obj.PageNumber > 0 ? obj.PageNumber - 1 : 0;
                var pageSize = obj.PageSize;

                var employees = _EmployeesRepo.GetAll(true);
                var result = employees.Select(e => GetEmployeeViewModel(e, false));
              
                Filtering<EmployeeViewModel> filtering = new Filtering<EmployeeViewModel>();
                filtering.Columns = obj.FilteredColumns;

                result = filtering.Search(result);

                //Sorting    
                result = filtering.OrderBy(obj.OrderBy, result);

                int totalRecords = result.Count();

                var data = result.Skip(pageIndex * pageSize).Take(pageSize).ToList();

                return Json(new { TotalCount = totalRecords, Data = data });
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
                return Json(new { TotalCount = 0, Data = string.Empty });
            }
        }
    }
}
