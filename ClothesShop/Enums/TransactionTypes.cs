using System.ComponentModel.DataAnnotations;

namespace ClothesShop.Enums
{
    public enum TransactionTypes
    {
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Incoming))]
        Incoming = 1,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Returned))]
        Returned = 2,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Lossses))]
        Lossses = 3,

    }
}