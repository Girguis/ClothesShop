using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClothesShop.Helpers;

namespace ClothesShop.Models
{
    public class TodayExpenseViewModel
    {
        public long ID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOn_ { get { return CreatedOn.ToString(DateTimeFormatter.DateFormat); } }
        public string CreatedBy { get; set; }

        public List<ExpensesViewModel> Expenses { get; set; }
        public ExpensesViewModel Expense { get; set; }
        public double TodayTotalExpensesCost { get; set; }

        public string TodayExpenseSeralized { get; set; }
        public bool IsApproved { get; set; }
    }
    public class ExpensesViewModel
    {
        public long ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Name))]
        [MaxLength(200, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.MustBeLessThan))]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Cost))]
        [RegularExpression(@"^([0-9.]+)$", ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.NumbersOnly))]
        public double Cost { get; set; }
        public int? TodayExpensesID { get; set; }
    }
}