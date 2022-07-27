using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClothesShop.Models
{
    public class EmployeeStatisticsViewModel
    {
        public long SellerID { get; set; }
        public string SellerName { get; set; }
        public int New { get; set; }
        public int Waiting { get; set; }
        public int CanceledByAgent { get; set; }
        public int TotallyDelivered { get; set; }
        public int PartialyDelivered { get; set; }
        public int Total { get; set; }

        
    }
}