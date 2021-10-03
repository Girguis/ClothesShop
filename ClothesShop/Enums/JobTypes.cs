using System.ComponentModel.DataAnnotations;

namespace ClothesShop.Enums
{
    public enum JobTypes
    {
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Manager))]
        Manager = 1,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Worker))]
        Worker = 2,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.DeliveryMan))]
        DeliveryMan = 3,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Employee))]
        Employee = 4,
    }
}