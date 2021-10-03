using System;
using System.Collections.Generic;
using System.Data.Entity;
using DAL;
using Logging.Services;

namespace BLL.Repositories
{
    public class SizesRepo : IRepository<Size>
    {
        readonly ClothesShopEntities ClothesShopEntities;
        public SizesRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public SizesRepo(ClothesShopEntities clothesShopEntities)
        {
            ClothesShopEntities = clothesShopEntities;
        }
        public Size Add(Size obj)
        {
            try
            {
                ClothesShopEntities.Sizes.Add(obj);
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
                Size c = GetByID(id);
                if (c != null && (c.ProductSizes == null || c.ProductSizes.Count <= 0))
                {
                    ClothesShopEntities.Sizes.Remove(c);
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

        public IEnumerable<Size> GetAll()
        {
            try
            {
                return ClothesShopEntities.Sizes;
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public Size GetByID(long id)
        {
            try
            {
                return ClothesShopEntities.Sizes.Find(id);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public bool Update(Size obj)
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
