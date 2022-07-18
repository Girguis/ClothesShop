﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ClothesShopEntities : DbContext
    {
        public ClothesShopEntities()
            : base("name=ClothesShopEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Expens> Expenses { get; set; }
        public virtual DbSet<JobRole> JobRoles { get; set; }
        public virtual DbSet<Login> Logins { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<ProductColor> ProductColors { get; set; }
        public virtual DbSet<ProductOrder> ProductOrders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductSize> ProductSizes { get; set; }
        public virtual DbSet<ProductSupplier> ProductSuppliers { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SalesRate> SalesRates { get; set; }
        public virtual DbSet<ShipmentCompany> ShipmentCompanies { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<TodayExpens> TodayExpenses { get; set; }
        public virtual DbSet<TodayTransaction> TodayTransactions { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<EmployeesBalance> EmployeesBalances { get; set; }
    
        public virtual int UpdateProduct(Nullable<int> iD, string name, Nullable<double> originalPrice)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            var nameParameter = name != null ?
                new ObjectParameter("Name", name) :
                new ObjectParameter("Name", typeof(string));
    
            var originalPriceParameter = originalPrice.HasValue ?
                new ObjectParameter("OriginalPrice", originalPrice) :
                new ObjectParameter("OriginalPrice", typeof(double));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateProduct", iDParameter, nameParameter, originalPriceParameter);
        }
    
        public virtual int AddColor(string color, ObjectParameter iD)
        {
            var colorParameter = color != null ?
                new ObjectParameter("Color", color) :
                new ObjectParameter("Color", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AddColor", colorParameter, iD);
        }
    
        public virtual int DeleteSystemData()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteSystemData");
        }
    
        public virtual ObjectResult<OrderViewModel> GetOrders(Nullable<int> orderID, string requestDate, string customerName, string customerNumber, Nullable<int> orderStatus, string sellerName, string deliveryName, string orderBy, string orderDirection, Nullable<int> pageNumber, Nullable<int> pageSize, Nullable<int> employeeID, ObjectParameter totalCount)
        {
            var orderIDParameter = orderID.HasValue ?
                new ObjectParameter("OrderID", orderID) :
                new ObjectParameter("OrderID", typeof(int));
    
            var requestDateParameter = requestDate != null ?
                new ObjectParameter("RequestDate", requestDate) :
                new ObjectParameter("RequestDate", typeof(string));
    
            var customerNameParameter = customerName != null ?
                new ObjectParameter("CustomerName", customerName) :
                new ObjectParameter("CustomerName", typeof(string));
    
            var customerNumberParameter = customerNumber != null ?
                new ObjectParameter("CustomerNumber", customerNumber) :
                new ObjectParameter("CustomerNumber", typeof(string));
    
            var orderStatusParameter = orderStatus.HasValue ?
                new ObjectParameter("OrderStatus", orderStatus) :
                new ObjectParameter("OrderStatus", typeof(int));
    
            var sellerNameParameter = sellerName != null ?
                new ObjectParameter("SellerName", sellerName) :
                new ObjectParameter("SellerName", typeof(string));
    
            var deliveryNameParameter = deliveryName != null ?
                new ObjectParameter("DeliveryName", deliveryName) :
                new ObjectParameter("DeliveryName", typeof(string));
    
            var orderByParameter = orderBy != null ?
                new ObjectParameter("OrderBy", orderBy) :
                new ObjectParameter("OrderBy", typeof(string));
    
            var orderDirectionParameter = orderDirection != null ?
                new ObjectParameter("OrderDirection", orderDirection) :
                new ObjectParameter("OrderDirection", typeof(string));
    
            var pageNumberParameter = pageNumber.HasValue ?
                new ObjectParameter("PageNumber", pageNumber) :
                new ObjectParameter("PageNumber", typeof(int));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("PageSize", pageSize) :
                new ObjectParameter("PageSize", typeof(int));
    
            var employeeIDParameter = employeeID.HasValue ?
                new ObjectParameter("EmployeeID", employeeID) :
                new ObjectParameter("EmployeeID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<OrderViewModel>("GetOrders", orderIDParameter, requestDateParameter, customerNameParameter, customerNumberParameter, orderStatusParameter, sellerNameParameter, deliveryNameParameter, orderByParameter, orderDirectionParameter, pageNumberParameter, pageSizeParameter, employeeIDParameter, totalCount);
        }
    
        public virtual ObjectResult<Nullable<int>> GetOrdersCount(Nullable<int> orderID, string requestDate, string customerName, string customerNumber, Nullable<int> orderStatus, string sellerName, string deliveryName, Nullable<int> employeeID)
        {
            var orderIDParameter = orderID.HasValue ?
                new ObjectParameter("OrderID", orderID) :
                new ObjectParameter("OrderID", typeof(int));
    
            var requestDateParameter = requestDate != null ?
                new ObjectParameter("RequestDate", requestDate) :
                new ObjectParameter("RequestDate", typeof(string));
    
            var customerNameParameter = customerName != null ?
                new ObjectParameter("CustomerName", customerName) :
                new ObjectParameter("CustomerName", typeof(string));
    
            var customerNumberParameter = customerNumber != null ?
                new ObjectParameter("CustomerNumber", customerNumber) :
                new ObjectParameter("CustomerNumber", typeof(string));
    
            var orderStatusParameter = orderStatus.HasValue ?
                new ObjectParameter("OrderStatus", orderStatus) :
                new ObjectParameter("OrderStatus", typeof(int));
    
            var sellerNameParameter = sellerName != null ?
                new ObjectParameter("SellerName", sellerName) :
                new ObjectParameter("SellerName", typeof(string));
    
            var deliveryNameParameter = deliveryName != null ?
                new ObjectParameter("DeliveryName", deliveryName) :
                new ObjectParameter("DeliveryName", typeof(string));
    
            var employeeIDParameter = employeeID.HasValue ?
                new ObjectParameter("EmployeeID", employeeID) :
                new ObjectParameter("EmployeeID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GetOrdersCount", orderIDParameter, requestDateParameter, customerNameParameter, customerNumberParameter, orderStatusParameter, sellerNameParameter, deliveryNameParameter, employeeIDParameter);
        }
    }
}
