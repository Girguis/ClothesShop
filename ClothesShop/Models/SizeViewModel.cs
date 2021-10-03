using System.ComponentModel.DataAnnotations;

namespace ClothesShop.Models
{
    public class SizeViewModel
    {
        public long ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Name))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.MustBeLessThan))]
        public string Name { get; set; }
    }
}