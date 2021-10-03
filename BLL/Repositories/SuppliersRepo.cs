using System;
using System.Collections.Generic;
using System.Data.Entity;
using DAL;
using Logging.Services;

namespace BLL.Repositories
{
    public class SuppliersRepo : IRepository<Supplier>
    {
        readonly ClothesShopEntities ClothesShopEntities;
        public SuppliersRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public SuppliersRepo(ClothesShopEntities clothesShopEntities)
        {
            ClothesShopEntities = clothesShopEntities;
        }
        public Supplier Add(Supplier obj)
        {
            try
            {
                ClothesShopEntities.Suppliers.Add(obj);
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
                Supplier c = GetByID(id);
                if (c != null && (c.ProductSuppliers == null || c.ProductSuppliers.Count <= 0))
                {
                    ClothesShopEntities.Suppliers.Remove(c);
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

        public IEnumerable<Supplier> GetAll()
        {
            try
            {
                return ClothesShopEntities.Suppliers;
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public Supplier GetByID(long id)
        {
            try
            {
                return ClothesShopEntities.Suppliers.Find(id);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public bool Update(Supplier obj)
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
