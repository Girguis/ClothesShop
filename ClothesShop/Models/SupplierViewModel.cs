using System.ComponentModel.DataAnnotations;

namespace ClothesShop.Models
{
    public class SupplierViewModel
    {
        public long ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Name))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.MobileNumber1))]
        [RegularExpression(@"^([0-9]+)$", ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.NumbersOnly))]
        [StringLength(11, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.EqualText), MinimumLength = 11)]

        public string MobileNumber1 { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.MobileNumber2))]
        [RegularExpression(@"^([0-9]+)$", ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.NumbersOnly))]
        [StringLength(11, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.EqualText), MinimumLength = 0)]

        public string MobileNumber2 { get; set; }

    }
}