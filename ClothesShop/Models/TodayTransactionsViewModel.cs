using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClothesShop.Helpers;

namespace ClothesShop.Models
{
    public class TransactionsSearchViewModel
    {
        [DataType(DataType.Date)]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.From))]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.To))]
        public DateTime EndDate { get; set; }
    }
    public class TransactionsViewModel
    {
        public string ProductName { get; set; }
        public int DayOne { get; set; }
        public int DayTwo { get; set; }
        public int DayThree { get; set; }
        public int DayFour { get; set; }
        public int DayFive { get; set; }
        public int DaySix { get; set; }
        public int DaySeven { get; set; }
        public int TotalDays { get { return DayOne + DayTwo + DayThree + DayFour + DayFive + DaySix + DaySeven; } }
    }



    public class TodayTransactionsViewModel
    {
        public long ID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_ { get { return CreatedOn.ToString(DateTimeFormatter.DateFormat); } }
        public string CreatedBy { get; set; }

        public List<TransactionViewModel> Transactions { get; set; }
        public TransactionViewModel Transaction { get; set; }
        public double TodayTotalTransactionsSellingPrice { get; set; }

        public string TodaySalesSeralized { get; set; }
        public bool IsApproved { get; set; }
    }
    public class TransactionViewModel
    {
        public long ID { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        //[Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Employee))]
        //public long? EmployeeID { get; set; } 

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Product))]
        public long? ProductID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.NumberOfPieces))]
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.MustBeGreaterThan))]
        public int? NumberOfPieces { get; set; }

        [DataType(DataType.Currency)]
        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.SellingPrice))]
        [RegularExpression(@"^([0-9.]+)$", ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.NumbersOnly))]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.MustBeGreaterThan))]
        public double SellingPrice { get; set; }

        //[Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Notes))]
        //public string Notes { get; set; }

        public long TodayTransactionID { get; set; }

        //public string EmployeeName { get; set; }
        public string ProductName { get; set; }
    }

}