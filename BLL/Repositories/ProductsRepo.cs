using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DAL;
using Logging.Services;

namespace BLL.Repositories
{
    public class ProductsRepo : IRepository<Product>
    {
        readonly ClothesShopEntities ClothesShopEntities;
        public ProductsRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public ProductsRepo(ClothesShopEntities clothesShopEntities)
        {
            ClothesShopEntities = clothesShopEntities;
        }
        public Product Add(Product obj)
        {
            try
            {
                ClothesShopEntities.Products.Add(obj);
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
                Product c = GetByID(id);
                c.IsEnabled = false;
                ClothesShopEntities.Entry(c).State = EntityState.Modified;
                int res = ClothesShopEntities.SaveChanges();
                return (res > 0);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
            }
            return false;
        }

        public IEnumerable<Product> GetAll()
        {
            try
            {
                return ClothesShopEntities.Products.Where(x=>x.IsEnabled == true);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public Product GetByID(long id)
        {
            try
            {
                return ClothesShopEntities.Products.Find(id);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public bool Update(Product obj)
        {
            try
            {
                Product p = GetByID(obj.ID);

                var oldColors = p.ProductColors;
                var newColors = obj.ProductColors;

                var colorsToDelete = oldColors.Where(c => !newColors.Select(x => x.ColorID).Contains(c.ColorID)).ToList();
                var colorsToAdd = newColors.Where(c => !oldColors.Select(x => x.ColorID).Contains(c.ColorID)).ToList();
                if (colorsToDelete != null && colorsToDelete.Count > 0)
                {
                    ClothesShopEntities.ProductColors.RemoveRange(colorsToDelete);
                    foreach (var color in colorsToDelete)
                        p.ProductColors.Remove(color);
                }


                if (colorsToAdd != null && colorsToAdd.Count > 0)
                {
                    ClothesShopEntities.ProductColors.AddRange(colorsToAdd);
                    foreach (var color in colorsToAdd)
                        p.ProductColors.Add(color);
                }



                var oldSizes = p.ProductSizes;
                var newSizes = obj.ProductSizes;

                var sizesToDelete = oldSizes.Where(c => !newSizes.Select(x => x.SizeID).Contains(c.SizeID)).ToList();
                var sizesToAdd = newSizes.Where(c => !oldSizes.Select(x => x.SizeID).Contains(c.SizeID)).ToList();
                if (sizesToDelete != null && sizesToDelete.Count > 0)
                {
                    ClothesShopEntities.ProductSizes.RemoveRange(sizesToDelete);
                    foreach (var size in sizesToDelete)
                        p.ProductSizes.Remove(size);
                }

                if (sizesToAdd != null && sizesToAdd.Count > 0)
                {
                    ClothesShopEntities.ProductSizes.AddRange(sizesToAdd);
                    foreach (var size in sizesToAdd)
                        p.ProductSizes.Add(size);
                }

                p.Name = obj.Name;
                p.OrginalPrice = obj.OrginalPrice;

                ClothesShopEntities.Entry(p).State = EntityState.Modified;
                int res = ClothesShopEntities.SaveChanges();
                return res > 0;
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return false;
            }
        }
    }
}
