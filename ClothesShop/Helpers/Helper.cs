using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using BLL.Repositories;
using ClothesShop.Enums;
using ClothesShop.Models;
using Logging.Services;

namespace ClothesShop.Helpers
{
    public static class Helper
    {
        //  private static string connectionString = ConfigurationManager.ConnectionStrings["ClothesShop"].ConnectionString;
        public static bool IsRtl()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;
        }
        public static List<General> EnumToList<T>()
        {
            ResourceManager resourceManager = new ResourceManager(typeof(Languages.Resources));
            var values = Enum.GetValues(typeof(T)).Cast<int>();
            var results = from val in values
                          select new General()
                          {
                              ID = val,
                              Name = resourceManager.GetString(Enum.GetName(typeof(T), val), CultureInfo.CurrentCulture)
                          };

            return results.ToList();
        }
        public static List<ColorViewModel> GetColors()
        {
            try
            {
                ColorRepo repo = new ColorRepo();
                var colors = repo.GetAll();
                if (colors != null && colors.Count() > 0)
                    return colors.Select(c => new ColorViewModel() { ID = c.ID, Name = c.Color1 }).ToList();
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }
        public static List<SizeViewModel> GetSizes()
        {
            try
            {
                SizesRepo repo = new SizesRepo();
                var sizes = repo.GetAll();
                if (sizes != null && sizes.Count() > 0)
                    return sizes.Select(c => new SizeViewModel() { ID = c.ID, Name = c.Size1 }).ToList();
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }
        public static List<General> GetCities()
        {
            try
            {
                CitiesRepo repo = new CitiesRepo();
                var cities = repo.GetAll();
                return cities.Select(j => new General() { ID = j.ID, Name = j.Name }).ToList();
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }
        public static List<General> GetSuppliers()
        {
            try
            {
                SuppliersRepo repo = new SuppliersRepo();

                var suppliers = repo.GetAll();
                return suppliers.Select(j => new General() { ID = j.ID, Name = j.Name }).ToList();
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }
        public static List<General> GetEmployees()
        {
            try
            {

                EmployeesRepo repo = new EmployeesRepo();

                var sellers = repo.GetAll(true).Where(c => c.JobTypeID == (int)JobTypes.Seller || c.JobTypeID == (int)JobTypes.PageOneSeller || c.JobTypeID == (int)JobTypes.PageTwoSeller);
                var res = sellers.Select(j => new General() { ID = j.ID, Name = j.FullName }).ToList();
                return res;
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }
        public static List<General> GetDeliveryMen()
        {
            try
            {
                EmployeesRepo repo = new EmployeesRepo();

                var deliveryMen = repo.GetAll(true).Where(c => c.JobTypeID == (int)JobTypes.DeliveryMan);
                var res = deliveryMen.Select(j => new General() { ID = j.ID, Name = j.FullName }).ToList();
                res.Insert(0, new General() { ID = 0, Name = Languages.Resources.NoDeliveryMan });
                return res;
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }
        public static List<General> GetProducts()
        {
            try
            {
                ProductsRepo repo = new ProductsRepo();

                var products = repo.GetAll();
                return products.Select(j => new General() { ID = j.ID, Name = j.Name }).ToList();
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }
        public static List<General> GetShipmentCompanies()
        {
            try
            {
                ShipmentCompanyRepo repo = new ShipmentCompanyRepo();

                var shipmentCompanies = repo.GetAll();
                var res = shipmentCompanies.Select(j => new General() { ID = j.ID, Name = j.Name }).ToList();
                res.Insert(0, new General() { ID = 0, Name = Languages.Resources.WithoutShipmentCompany });
                return res;
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.PresentationLayer, ex);
            }
            return null;
        }
    }
    public class General
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }
}