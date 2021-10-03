using System.ComponentModel.DataAnnotations;

namespace ClothesShop.Enums
{
    public enum Gender
    {
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Male))]
        Male = 1,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Female))]
        Female = 2
    }
}