using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ClothesShop.Helpers;

namespace ClothesShop.Models
{
    public class OrdersViewModel
    {
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Code))]
        public long ID { get; set; }
        public long? CustomerID { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.DeliveryMan))]
        public long? EmployeeID { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Seller))]
        public long? SellerID { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.ShipmentCompany))]
        public long? ShipmentCompanyID { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.OrderStatus))]
        public int OrderStatusID { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.City))]
        public int CityID { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.ShipmentPrice))]
        public double? ShipmentPrice { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.PaidAmount))]
        public double PaidAmount { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Notes))]
        public string Notes { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.DeliveryDate))]
        [DataType(DataType.Date)]
        public DateTime? DeliveryDate { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.RequestDate))]
        [DataType(DataType.Date)]
        public DateTime? RequestDate { get; set; }
        public string RequestDate_
        {
            get
            {
                return RequestDate.HasValue? RequestDate.Value.ToString(DateTimeFormatter.DateFormat + " " + DateTimeFormatter.TimeFormat):"";
            }
        }
        public CustomerViewModel Customer { get; set; }

       // [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
       // [MinLength(1)]
        public List<ProductsViewModel> Products { get; set; }
        public ProductsViewModel Product { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Total))]
        public double OrderTotalPrice { get; set; }
        public string OrderStatusName { get; set; }
        public string EmployeeName { get; set; }
        public string SellerName { get; set; }
        public string CityName { get; set; }
        public string ShipmentCompanyName { get; set; }
    }
    public class CustomerViewModel
    {
        public long ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.CustomerName))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Address))]
        public string Address { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.MobileNumber1))]
        [RegularExpression(@"^([0-9]+)$", ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.NumbersOnly))]
        [StringLength(11, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.EqualText), MinimumLength = 11)]

        public string MobileNumber1 { get; set; }

        // [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.MobileNumber2))]
        [RegularExpression(@"^([0-9]+)$", ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.NumbersOnly))]
        [StringLength(11, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.EqualText), MinimumLength = 0)]

        public string MobileNumber2 { get; set; }
    }
    public class ProductsViewModel
    {
        public long ID { get; set; }
        public long ProductID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Product))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.NumberOfPieces))]
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.MustBeGreaterThan))]
        public int? NumberOfPieces { get; set; }

        [DataType(DataType.Currency)]
        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.PiecePrice))]
        public double OriginalPrice { get; set; }

        [DataType(DataType.Currency)]
        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.PiecePrice))]

        public double SellingPrice { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Color))]
        public long ColorID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Size))]
        public long SizeID { get; set; }

        public string ColorName { get; set; }
        public string SizeName { get; set; }
    }

}