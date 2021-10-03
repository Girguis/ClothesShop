using System.ComponentModel.DataAnnotations;

namespace ClothesShop.Enums
{
    public enum OrderStatuses
    {
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.New))]
        New = 0,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Waiting))]
        Waiting = 1,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.PartialyDelivered))]
        PartialyDelivered = 2,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.TotallyDelivered))]
        TotallyDelivered = 3,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Delayed))]
        Delayed = 4,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.NotDelivered))]
        NotDelivered = 5,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.CanceledByAgent))]
        CanceledByAgent = 6,
    }
}