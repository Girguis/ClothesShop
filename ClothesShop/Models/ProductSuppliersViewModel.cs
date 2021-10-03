using System;
using System.ComponentModel.DataAnnotations;
using ClothesShop.Helpers;

namespace ClothesShop.Models
{
    public class ProductSuppliersViewModel
    {
        public long ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.TransactionType))]
        public long? TransactionTypeID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Product))]
        public long? ProductID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Supplier))]
        public long? SupplierID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.NumberOfPieces))]
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.MustBeGreaterThan))]
        public int? NumberOfPieces { get; set; }

        [DataType(DataType.Currency)]
        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.OriginalPrice))]
        public double? OrginalPrice { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.CreatedOn))]
        public DateTime? CreatedOn { get; set; }

        public string CreatedOn_
        {
            get
            {
                var dateTimeFormatter = DateTimeFormatter.DateFormat + " " + DateTimeFormatter.TimeFormat;
           
                if (CreatedOn.HasValue)
                    return CreatedOn.Value.ToString(dateTimeFormatter);
                return "";
            }
        }
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.CreatedBy))]
        public string CreatedBy { get; set; }
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Product))]
        public string ProductName { get; set; }
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Supplier))]
        public string SupplierName { get; set; }
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.TransactionType))]
        public string TransactionName { get; set; }
    }
}