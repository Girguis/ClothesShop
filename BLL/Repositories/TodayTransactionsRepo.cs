using System;
using System.Collections.Generic;
using System.Linq;
using DAL;

namespace BLL.Repositories
{
    public class TodayTransactionsRepo : IRepository<TodayTransaction>
    {
        readonly ClothesShopEntities ClothesShopEntities;
        public TodayTransactionsRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public TodayTransaction Add(TodayTransaction obj)
        {
            try
            {
                ClothesShopEntities.TodayTransactions.Add(obj);
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
                TodayTransaction TodayTransaction = GetByID(id);

                ClothesShopEntities.Transactions.RemoveRange(TodayTransaction.Transactions);

                ClothesShopEntities.TodayTransactions.Remove(TodayTransaction);
                int result = ClothesShopEntities.SaveChanges();
                return result > 0;
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return false;
            }
        }

        public IEnumerable<TodayTransaction> GetAll()
        {
            return ClothesShopEntities.TodayTransactions;
        }

        public TodayTransaction GetByID(long id)
        {
            return ClothesShopEntities.TodayTransactions.Find(id);
        }

        public bool Update(TodayTransaction obj)
        {
            try
            {
                TodayTransaction old = GetByID(obj.ID);
                old.IsApproved = obj.IsApproved;

                var deletedTransactions = old.Transactions.Where(c => !obj.Transactions.Select(x => x.ID).Contains(c.ID)).ToList();
                var addedTransactions = obj.Transactions.Where(c => !old.Transactions.Select(x => x.ID).Contains(c.ID)).ToList();

                if (deletedTransactions != null && deletedTransactions.Count > 0)
                {
                    ClothesShopEntities.Transactions.RemoveRange(deletedTransactions);
                    foreach (var ex in deletedTransactions)
                        old.Transactions.Remove(ex);
                }
                if (addedTransactions != null && addedTransactions.Count > 0)
                {
                    ClothesShopEntities.Transactions.AddRange(addedTransactions);
                    foreach (var ex in addedTransactions)
                        old.Transactions.Add(ex);
                }
                var updatedTransactions = obj.Transactions.Where(e => e.ID != 0 && !deletedTransactions.Select(c => c.ID).Contains(e.ID)
                 && (old.Transactions.First(w => w.ID == e.ID).EmployeeID != e.EmployeeID
                     || old.Transactions.First(w => w.ID == e.ID).Notes != e.Notes
                     || old.Transactions.First(w => w.ID == e.ID).NumberOfPieces != e.NumberOfPieces
                     || old.Transactions.First(w => w.ID == e.ID).SellingPrice != e.SellingPrice
                     || old.Transactions.First(w => w.ID == e.ID).ProductID != e.ProductID

                     )).ToList();

                if (updatedTransactions != null && updatedTransactions.Count > 0)
                {
                    foreach (var ex in updatedTransactions)
                    {
                        old.Transactions.First(c => c.ID == ex.ID).EmployeeID = ex.EmployeeID;
                        old.Transactions.First(c => c.ID == ex.ID).Notes = ex.Notes;
                        old.Transactions.First(c => c.ID == ex.ID).NumberOfPieces = ex.NumberOfPieces;
                        old.Transactions.First(c => c.ID == ex.ID).SellingPrice = ex.SellingPrice;
                        old.Transactions.First(c => c.ID == ex.ID).ProductID = ex.ProductID;
                        old.Transactions.First(c => c.ID == ex.ID).TodayTransactionID = obj.ID;
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
