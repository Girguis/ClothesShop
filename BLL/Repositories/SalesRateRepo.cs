using System;
using System.Collections.Generic;
using System.Data.Entity;
using DAL;
using Logging.Services;

namespace BLL.Repositories
{
    public class SalesRatesRepo : IRepository<SalesRate>
    {
        readonly ClothesShopEntities ClothesShopEntities;
        public SalesRatesRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public SalesRatesRepo(ClothesShopEntities clothesShopEntities)
        {
            ClothesShopEntities = clothesShopEntities;
        }
        public SalesRate Add(SalesRate obj)
        {
            try
            {
                ClothesShopEntities.SalesRates.Add(obj);
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
                SalesRate c = GetByID(id);
                ClothesShopEntities.SalesRates.Remove(c);
                int res = ClothesShopEntities.SaveChanges();
                return (res > 0);

            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
            }
            return false;
        }

        public IEnumerable<SalesRate> GetAll()
        {
            try
            {
                return ClothesShopEntities.SalesRates;
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public SalesRate GetByID(long id)
        {
            try
            {
                return ClothesShopEntities.SalesRates.Find(id);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public bool Update(SalesRate obj)
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
