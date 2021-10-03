using System;
using System.Collections.Generic;
using System.Linq;
using DAL;

namespace BLL.Repositories
{
    public class AccountRepo:IRepository<Login>
    {
        private readonly ClothesShopEntities ClothesShopEntities;
        public AccountRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public AccountRepo(ClothesShopEntities clothesShopEntities)
        {
            ClothesShopEntities = clothesShopEntities;
        }
        public bool IsExists(Login model)
        {
            return ClothesShopEntities.Logins.Any(l => l.UserName.ToLower() == model.UserName.ToLower() && l.Password == model.Password);
        } 
        public bool IsUserNameExists(long userId , string userName)
        {
            return ClothesShopEntities.Logins.Any(l => l.UserName.ToLower() == userName.ToLower() && l.EmployeeID != userId);
        }
        public Login Get(Login model)
        {
            return ClothesShopEntities.Logins.Where(l => l.UserName.ToLower() == model.UserName.ToLower() && l.Password == model.Password).FirstOrDefault();
        }

        public IEnumerable<Login> GetAll()
        {
            return ClothesShopEntities.Logins;
        }

        public Login GetByID(long id)
        {
            return ClothesShopEntities.Logins.Find(id); 
        }

        public Login Add(Login obj)
        {
            try
            {
                ClothesShopEntities.Logins.Add(obj);
                ClothesShopEntities.SaveChanges();
            }
            catch(Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
            }
            return obj;
        }

        public bool Update(Login obj)
        {
            try
            {
                ClothesShopEntities.Entry(obj).State =System.Data.Entity.EntityState.Modified;
                int result = ClothesShopEntities.SaveChanges();
                return (result > 0);
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
            }
            return false;
        }

        public bool Delete(long id)
        {
            try
            {
                Login obj = GetByID(id);
                ClothesShopEntities.Logins.Remove(obj);
                int result = ClothesShopEntities.SaveChanges();
                return (result > 0);
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
            }
            return false;
        }
        public bool DeleteSystemData()
        {
            try
            {
                ClothesShopEntities.DeleteSystemData();
                return true;
            }
            catch(Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
            }
            return false;
        }

    }
}
