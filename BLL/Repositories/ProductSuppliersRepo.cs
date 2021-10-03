using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DAL;
using Logging.Services;

namespace BLL.Repositories
{
    public class ProductSuppliersRepo : IRepository<ProductSupplier>
    {
        private readonly ClothesShopEntities ClothesShopEntities;
        public ProductSuppliersRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public ProductSuppliersRepo(ClothesShopEntities clothesShopEntities)
        {
            ClothesShopEntities = clothesShopEntities;
        }
        public ProductSupplier Add(ProductSupplier obj)
        {
            ClothesShopEntities.ProductSuppliers.Add(obj);
            ClothesShopEntities.SaveChanges();
            return obj;
        }

        public bool Delete(long id)
        {
            try
            {
                ProductSupplier productSupplier = GetByID(id);
                if (productSupplier == null)
                    return false;
                ClothesShopEntities.ProductSuppliers.Remove(productSupplier);
                int res = ClothesShopEntities.SaveChanges();
                return (res > 0);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return false;
            }
        }
        public IEnumerable<ProductSupplier> GetAll()
        {
            try
            {
                return ClothesShopEntities.ProductSuppliers;
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public ProductSupplier GetByID(long id)
        {
            try
            {
                return ClothesShopEntities.ProductSuppliers.Where(p => p.ID == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public bool Update(ProductSupplier obj)
        {
            try
            {
                ClothesShopEntities.Entry(obj).State = EntityState.Modified;
                int res = ClothesShopEntities.SaveChanges();
                return (res > 0);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return false;
            }
        }
    }
}
