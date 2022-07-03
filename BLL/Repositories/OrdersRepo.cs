using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using DAL;

namespace BLL.Repositories
{
    public class OrdersRepo : IRepository<Order>
    {
        private readonly ClothesShopEntities ClothesShopEntities;
        public OrdersRepo()
        {
            ClothesShopEntities = new ClothesShopEntities();
        }
        public Order Add(Order obj)
        {
            try
            {
                ClothesShopEntities.Orders.Add(obj);
                ClothesShopEntities.SaveChanges();
            }
            catch(Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
            }
            return obj;
        }

        public bool Delete(long id)
        {
            try
            {
                Order order = GetByID(id);
                ClothesShopEntities.ProductOrders.RemoveRange(order.ProductOrders);
                ClothesShopEntities.Orders.Remove(order);
                int result = ClothesShopEntities.SaveChanges();
                return (result > 0);
            }
            catch(Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return false;
            }
        }

        public IEnumerable<Order> GetAll()
        {
            return ClothesShopEntities.Orders;
        }
        public IEnumerable<OrderViewModel> Get(int? OrderID,string RequestDate,string CustomerName,string CustomerNumber,
            int? OrderStatus,string SellerName,string DeliveryName,string OrderBy,string OrderDirection,
            int? PageNumber,int? PageSize,int? empID,out int totalCount_)
        {
            totalCount_ = 0;
            ObjectParameter TotalCount=new ObjectParameter("TotalCount", typeof(int));
            ObjectResult<OrderViewModel> result =  ClothesShopEntities.GetOrders(OrderID,RequestDate,CustomerName,CustomerNumber,OrderStatus,SellerName,DeliveryName,OrderBy,OrderDirection,PageNumber,PageSize, empID, TotalCount);
            ObjectResult<int?> res2 = ClothesShopEntities.GetOrdersCount(OrderID, RequestDate, CustomerName, CustomerNumber, OrderStatus, SellerName, DeliveryName, empID);
            List<int?> lst = res2.ToList();
            if (lst != null && lst.Count >= 1)
                totalCount_ = lst[0].Value;
            
            return result;// new List<OrderViewModel>();
        }
        public Order GetByID(long id)
        {
            return ClothesShopEntities.Orders.Find(id);
        }

        public bool Update(Order obj)
        {
            try
            {
                Order old = GetByID(obj.ID);

                var deletedProductOrders = old.ProductOrders.Where(c => !obj.ProductOrders.Select(x => x.ID).Contains(c.ID)).ToList();
                var addedProductOrders = obj.ProductOrders.Where(c => !old.ProductOrders.Select(x => x.ID).Contains(c.ID)).ToList();

                if (deletedProductOrders != null && deletedProductOrders.Count > 0)
                {
                    ClothesShopEntities.ProductOrders.RemoveRange(deletedProductOrders);
                    foreach (var ex in deletedProductOrders)
                        old.ProductOrders.Remove(ex);
                }
                if (addedProductOrders != null && addedProductOrders.Count > 0)
                {
                    ClothesShopEntities.ProductOrders.AddRange(addedProductOrders);
                    foreach (var ex in addedProductOrders)
                        old.ProductOrders.Add(ex);
                }
                //var updatedProductOrders = obj.ProductOrders.Where(e => e.ID != 0 && !deletedProductOrders.Select(c => c.ID).Contains(e.ID)
                // && (old.ProductOrders.First(w => w.ID == e.ID).OrginalPrice != e.OrginalPrice
                //     || old.ProductOrders.First(w => w.ID == e.ID).Quantity != e.Quantity
                //     || old.ProductOrders.First(w => w.ID == e.ID).SellingPrice != e.SellingPrice
                //     )).ToList();

                //if (updatedProductOrders != null && updatedProductOrders.Count > 0)
                //{
                //    foreach (var ex in updatedProductOrders)
                //    {
                //        old.ProductOrders.First(w => w.ID == ex.ID).OrginalPrice = ex.OrginalPrice;
                //        old.ProductOrders.First(c => c.ID == ex.ID).Quantity = ex.Quantity;
                //        old.ProductOrders.First(c => c.ID == ex.ID).SellingPrice = ex.SellingPrice;
                //    }
                //}
                old.Notes = obj.Notes;
                old.OrderStatusID = obj.OrderStatusID;
                old.PaidAmount = obj.PaidAmount;
                old.ShipmentCompanyID = obj.ShipmentCompanyID;
                old.ShipmentPrice = obj.ShipmentPrice;
                old.CityID = obj.CityID;
                old.Customer = obj.Customer;
                old.DeliveryDate = obj.DeliveryDate;
                old.EmployeeID = obj.EmployeeID;
                old.SellerID = obj.SellerID;
                ClothesShopEntities.Entry(obj.Customer).State = System.Data.Entity.EntityState.Modified;
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
        public bool AssignOrdersToDeliveryMan(long deliverManId , List<long> orderIds)
        {
            try {
                var orders = GetAll().Where(o => orderIds.Contains(o.ID)).ToList();
                foreach(var order in orders)
                {
                    order.EmployeeID = deliverManId;
                    order.OrderStatusID = 1;// waiting
                    ClothesShopEntities.Entry(order).State = System.Data.Entity.EntityState.Modified;
                }
                int result = ClothesShopEntities.SaveChanges();
                return result > 0;
            }
            catch (Exception ex)
            {
                Logging.Services.LogErrorService.Write(Logging.Enums.AppTypes.BLL, ex);
                return false;
            }
        }
        public bool ChangeOrderStatus(Order o)
        {
            try
            { 
                ClothesShopEntities.Entry(o).State = System.Data.Entity.EntityState.Modified;
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
