using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClothesShop.Enums
{
    public enum BalanceType
    {
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Balance))]
        Balance,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Withdraw))]
        Withdraw,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Reward))]
        Reward,
        [Display(ResourceType = typeof(Languages.Resources), Name = nameof(Languages.Resources.Deduction))]
        Deduction
    }
}