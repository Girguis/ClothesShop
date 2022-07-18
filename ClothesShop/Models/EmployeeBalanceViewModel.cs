using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClothesShop.Models
{
    public class EmployeeBalanceViewModel
    {
        public int ID { get; set; }
        public int SellerID { get; set; }
        public string SellerName { get; set; }
        public DateTime CreateDate { get; set; }
        [Required]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Amount))]
        public double Amount { get; set; }
        [Required]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.TransactionType))]
        public int Type { get; set; }

    }
}