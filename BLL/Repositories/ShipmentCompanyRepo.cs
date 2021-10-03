using System;
using System.Collections.Generic;
using System.Data.Entity;
using DAL;
using Logging.Services;

namespace BLL.Repositories
{
    public class ShipmentCompanyRepo : IRepository<ShipmentCompany>
    {
        readonly ClothesShopEntities ClothesShopEntities;
        public ShipmentCompanyRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public ShipmentCompanyRepo(ClothesShopEntities clothesShopEntities)
        {
            ClothesShopEntities = clothesShopEntities;
        }
        public ShipmentCompany Add(ShipmentCompany obj)
        {
            try
            {
                ClothesShopEntities.ShipmentCompanies.Add(obj);
                int res = ClothesShopEntities.SaveChanges();
                if (res > 0)
                    return obj;
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
            }
            return null;
        }

        public bool Delete(long id)
        {
            try
            {
                ShipmentCompany c = GetByID(id);
                if (c != null && (c.Orders == null || c.Orders.Count <= 0))
                {
                    ClothesShopEntities.ShipmentCompanies.Remove(c);
                    int res = ClothesShopEntities.SaveChanges();
                    return (res > 0);
                }

            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
            }
            return false;
        }

        public IEnumerable<ShipmentCompany> GetAll()
        {
            try
            {
                return ClothesShopEntities.ShipmentCompanies;
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public ShipmentCompany GetByID(long id)
        {
            try
            {
                return ClothesShopEntities.ShipmentCompanies.Find(id);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public bool Update(ShipmentCompany obj)
        {
            try
            {
                ClothesShopEntities.Entry(obj).State = EntityState.Modified;
                ClothesShopEntities.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return false;
            }
        }
    }
}
