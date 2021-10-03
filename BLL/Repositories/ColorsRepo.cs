using System;
using System.Collections.Generic;
using System.Data.Entity;
using DAL;
using Logging.Services;

namespace BLL.Repositories
{
    public class ColorRepo : IRepository<Color>
    {
        readonly ClothesShopEntities ClothesShopEntities;
        public ColorRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public ColorRepo(ClothesShopEntities clothesShopEntities)
        {
            ClothesShopEntities = clothesShopEntities;
        }
        public Color Add(Color obj)
        {
            try
            {
                ClothesShopEntities.Colors.Add(obj);
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
                Color c = GetByID(id);
                if (c != null && (c.ProductColors == null || c.ProductColors.Count <= 0))
                {
                    ClothesShopEntities.Colors.Remove(c);
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

        public IEnumerable<Color> GetAll()
        {
            try
            {
                return ClothesShopEntities.Colors;
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public Color GetByID(long id)
        {
            try
            {
                return ClothesShopEntities.Colors.Find(id);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public bool Update(Color obj)
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
