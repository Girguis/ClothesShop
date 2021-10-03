using System;
using System.Collections.Generic;
using System.Linq;
using DAL;

namespace BLL.Repositories
{
    public class TodayExpensesRepo : IRepository<TodayExpens>
    {
        readonly ClothesShopEntities ClothesShopEntities;
        public TodayExpensesRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public TodayExpens Add(TodayExpens obj)
        {
            try
            {
                ClothesShopEntities.TodayExpenses.Add(obj);
                ClothesShopEntities.SaveChanges();
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
                TodayExpens todayExpens = GetByID(id);

                ClothesShopEntities.Expenses.RemoveRange(todayExpens.Expenses);

                ClothesShopEntities.TodayExpenses.Remove(todayExpens);
                int result = ClothesShopEntities.SaveChanges();
                return result > 0;
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return false;
            }
        }

        public IEnumerable<TodayExpens> GetAll()
        {
            return ClothesShopEntities.TodayExpenses;
        }

        public TodayExpens GetByID(long id)
        {
            return ClothesShopEntities.TodayExpenses.Find(id);
        }

        public bool Update(TodayExpens obj)
        {
            try
            {
                TodayExpens old = GetByID(obj.ID);
                old.IsApproved = obj.IsApproved;

                var deletedExpenses = old.Expenses.Where(c => !obj.Expenses.Select(x => x.ID).Contains(c.ID)).ToList();
                var addedExpenses = obj.Expenses.Where(c => !old.Expenses.Select(x => x.ID).Contains(c.ID)).ToList();

                if (deletedExpenses != null && deletedExpenses.Count > 0)
                {
                    ClothesShopEntities.Expenses.RemoveRange(deletedExpenses);
                    foreach (var ex in deletedExpenses)
                        old.Expenses.Remove(ex);
                }
                if (addedExpenses != null && addedExpenses.Count > 0)
                {
                    ClothesShopEntities.Expenses.AddRange(addedExpenses);
                    foreach (var ex in addedExpenses)
                        old.Expenses.Add(ex);
                }
                var updatedExpenses = obj.Expenses.Where(e => e.ID != 0 && !deletedExpenses.Select(c => c.ID).Contains(e.ID)
                 && (old.Expenses.First(w => w.ID == e.ID).Name != e.Name || old.Expenses.First(w => w.ID == e.ID).Cost != e.Cost)).ToList();

                if (updatedExpenses != null && updatedExpenses.Count > 0)
                {
                    foreach(var ex in updatedExpenses)
                    {
                        old.Expenses.First(c => c.ID == ex.ID).Name = ex.Name;
                        old.Expenses.First(c => c.ID == ex.ID).Cost = ex.Cost;
                    }
                }

                ClothesShopEntities.Entry(old).State = System.Data.Entity.EntityState.Modified;
                int result = ClothesShopEntities.SaveChanges();
                return result > 0;
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return false;
            }
        }
    }
}
