using DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Repositories
{
    public class EmployeeBalanceRepo : IRepository<EmployeesBalance>
    {
        private readonly ClothesShopEntities clothesShopEntities;

        public EmployeeBalanceRepo()
        {
            clothesShopEntities = new ClothesShopEntities();
        }
        public EmployeeBalanceRepo(ClothesShopEntities clothesShopEntities)
        {
            this.clothesShopEntities = clothesShopEntities;
        }
        public EmployeesBalance Add(EmployeesBalance obj)
        {
            try
            {
                clothesShopEntities.EmployeesBalances.Add(obj);
                clothesShopEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
            }
            return obj;
        }

        public bool Delete(long id)
        {
            try
            {
                EmployeesBalance empBalance = clothesShopEntities.EmployeesBalances.Find(id);
                clothesShopEntities.EmployeesBalances.Remove(empBalance);
                int res = clothesShopEntities.SaveChanges();
                return (res > 0);
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return false;
            }
        }

        public IEnumerable<EmployeesBalance> GetAll()
        {
            return clothesShopEntities.EmployeesBalances;
        }

        public EmployeesBalance GetByID(long id)
        {
            return clothesShopEntities.EmployeesBalances.Find(id);
        }

        public bool Update(EmployeesBalance obj)
        {
            try
            {
                clothesShopEntities.Entry(obj).State = EntityState.Modified;
                clothesShopEntities.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return false;
            }
        }
    }
}
