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
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.Logins = new HashSet<Login>();
            this.Orders = new HashSet<Order>();
            this.Orders1 = new HashSet<Order>();
            this.Transactions = new HashSet<Transaction>();
            this.EmployeesBalances = new HashSet<EmployeesBalance>();
        }
    
        public long ID { get; set; }
        public string FullName { get; set; }
        public Nullable<System.DateTime> StartWorkingDate { get; set; }
        public string Address { get; set; }
        public Nullable<int> GenderID { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public string SSN { get; set; }
        public Nullable<double> Salary { get; set; }
        public string AdditionalInfo { get; set; }
        public string FrontSSNURL { get; set; }
        public string BackSSNURL { get; set; }
        public string FrontLicenceURL { get; set; }
        public string BackLicenceURL { get; set; }
        public string JobName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int JobTypeID { get; set; }
        public Nullable<int> CityID { get; set; }
        public string Language { get; set; }
        public string MobileNumber1 { get; set; }
        public string MobileNumber2 { get; set; }
        public Nullable<double> UtcOffset { get; set; }
    
        public virtual City City { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Login> Logins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeesBalance> EmployeesBalances { get; set; }
    }
}
