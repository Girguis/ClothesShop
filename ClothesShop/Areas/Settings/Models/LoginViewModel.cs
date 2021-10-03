using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ClothesShop.Areas.Settings.Models
{
    public class LoginViewModel
    {
        public long ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Employee))]
        public long EmployeeID { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.FullName))]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.UserName))]
        [Remote("IsUserNameExists", "Account", "", ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.UserNameAlreadyExists))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.MustBeLessThan))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Password))]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.ConfirmPassword))]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.PasswordAndConfirmNotMatched))]
        public string ConfirmPassword { get; set; }
    }
}