using System.ComponentModel.DataAnnotations;

namespace ClothesShop.Areas.Settings.Models
{
    public class JobRoleViewModel
    {
        public long ID { get; set; }
        public long RoleID { get; set; }
        public int JobTypeID { get; set; }

        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Add))]
        public bool Add { get; set; }
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Edit))]
        public bool Edit { get; set; }
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Delete))]
        public bool Delete { get; set; }
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.View))]
        public bool View { get; set; }
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Details))]
        public bool Details { get; set; }
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Rights))]
        public string Name { get; set; }
    }
}