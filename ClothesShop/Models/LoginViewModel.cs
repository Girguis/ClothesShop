using System.ComponentModel.DataAnnotations;

namespace ClothesShop.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.UserName))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.MustBeLessThan))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Password))]
        public string Password { get; set; }
    }
}