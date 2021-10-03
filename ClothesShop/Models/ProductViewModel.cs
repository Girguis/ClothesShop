using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClothesShop.Models
{
    public class ProductViewModel
    {
        public long ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Name))]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.OriginalPrice))]

        public double OriginalPrice { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Colors))]
        [MinLength(1)]
        public IList<string> ColorIDs { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Sizes))]
        [MinLength(1)]
        public IList<string> SizeIDs { get; set; }

        public IList<string> ColorNames { get; set; }
        public IList<string> SizeNames { get; set; }
    }
}