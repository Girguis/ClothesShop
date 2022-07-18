using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClothesShop.Models
{
    public class SingleSellerStaticticsViewModel
    {
        public long SellerID { get; set; }
        public string SellerName { get; set; }
        public int New { get; set; }
        public int Waiting { get; set; }
        public int TotallyDelivered { get; set; }
        public int PartialyDelivered { get; set; }
        public int CanceledByAgent { get; set; }
        public int TotalOrders { get; set; }
        public double TotalBalance { get; set; }
        public double TotalWithdraw { get; set; }
        public double TotalDeduction { get; set; }
        public double TotalReward { get; set; }
    }
}