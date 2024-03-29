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
    using System.Collections.Generic;
    
    public partial class ProductSupplier
    {
        public long ID { get; set; }
        public Nullable<long> TransactionTypeID { get; set; }
        public Nullable<long> ProductID { get; set; }
        public Nullable<long> SupplierID { get; set; }
        public Nullable<int> NumberOfPieces { get; set; }
        public Nullable<double> OrginalPrice { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
