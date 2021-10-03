using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using ClothesShop.Enums;

namespace ClothesShop.Models
{
    public class EmployeeViewModel
    {
        public long ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.FullName))]
        [MaxLength(100, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.MustBeLessThan))]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Address))]
        public string Address { get; set; }


        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.City))]
        public int? CityID { get; set; }

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

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.StartWorkingDate))]
        [DataType(DataType.Date)]
        public DateTime? StartWorkingDate { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Gender))]
        public int? GenderID { get; set; } = (int)Gender.Male;

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.BirthDate))]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.SSN))]
        [RegularExpression(@"[2-3]{1}\d{13}$", ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.InvalidSSN))]
        [StringLength(14, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.EqualText), MinimumLength = 14)]
        public string SSN { get; set; }

        [DataType(DataType.Currency)]
        //[Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Salary))]

        public double? Salary { get; set; }

        // [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.AdditionalInfo))]
        public string AdditionalInfo { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.JobName))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.MustBeLessThan))]
        public string JobName { get; set; }

        // [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.JobType))]
        public int? JobTypeID { get; set; } = 4;

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.IsActive))]
        public bool? IsActive { get; set; } = true;

        //  [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        //  [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.FrontSSNPicture))]
        public string FrontSSNURL { get; set; }

        // [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        //  [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.BackSSNPicture))]
        public string BackSSNURL { get; set; }

        // [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        // [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.FrontLicencePicture))]
        public string FrontLicenceURL { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        //[Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.BackLicencePicture))]
        public string BackLicenceURL { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.BackLicencePicture))]
        public HttpPostedFileBase BackLicence { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.FrontLicencePicture))]
        public HttpPostedFileBase FrontLicence { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.BackSSNPicture))]
        public HttpPostedFileBase BackSSN { get; set; }

        [Required(ErrorMessageResourceType = typeof(Languages.Resources), ErrorMessageResourceName = nameof(Languages.Resources.RequiredField))]
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.FrontSSNPicture))]
        public HttpPostedFileBase FrontSSN { get; set; }
    }
}