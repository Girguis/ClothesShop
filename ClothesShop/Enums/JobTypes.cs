using System.ComponentModel.DataAnnotations;

namespace ClothesShop.Enums
{
    public enum JobTypes
    {
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Manager))]
        Manager = 1,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Seller))]
        Seller = 2,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.DeliveryMan))]
        DeliveryMan = 3,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.PageOneSeller))]
        PageOneSeller = 4,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.PageTwoSeller))]
        PageTwoSeller = 5,
    }
}