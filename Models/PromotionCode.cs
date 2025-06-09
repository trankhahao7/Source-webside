using System;
using System.Collections.Generic;

namespace PBL3_MicayOnline.Models;

public partial class PromotionCode
{
    public int PromoCodeId { get; set; }

    public string Code { get; set; } = null!;

    public decimal Discount { get; set; }

    public DateTime ExpiryDate { get; set; }

    public int? MaxUsage { get; set; }

    public int? UsedCount { get; set; }

    public decimal? MinOrderValue { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
