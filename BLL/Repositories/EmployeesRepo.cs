using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DAL;
using Logging.Services;

namespace BLL.Repositories
{
    public class EmployeesRepo : IRepository<Employee>
    {
        readonly ClothesShopEntities ClothesShopEntities;
        public EmployeesRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public EmployeesRepo(ClothesShopEntities clothesShopEntities)
        {
            ClothesShopEntities = clothesShopEntities;
        }
        public Employee Add(Employee obj)
        {
            try
            {
                ClothesShopEntities.Employees.Add(obj);
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
                Employee e = GetByID(id);
                e.IsActive = false;
                return Update(e);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
            }
            return false;
        }

        public IEnumerable<Employee> GetAll(bool isActive = true)
        {
            try
            {
                return ClothesShopEntities.Employees.Where(e => e.IsActive == isActive);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }
        public IEnumerable<Employee> GetAll()
        {
            throw new NotImplementedException();
        }

        public Employee GetByID(long id)
        {
            try
            {
                return ClothesShopEntities.Employees.Find(id);
            }
            catch (Exception ex)
            {
                LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return null;
            }
        }

        public bool Update(Employee obj)
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
