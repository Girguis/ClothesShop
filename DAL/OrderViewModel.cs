//------------------------------------------------------------------------------
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
    
    public partial class OrderViewModel
    {
        public long ID { get; set; }
        public System.DateTime RequestDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerAddress { get; set; }
        public int OrderStatusID { get; set; }
        public string DeliveryManName { get; set; }
        public string SellerName { get; set; }
        public double Total { get; set; }
        public double ShipmentPrice { get; set; }
        public string OrderStatusName { get; set; }
    }
}
