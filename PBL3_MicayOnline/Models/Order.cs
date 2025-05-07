using System;
using System.Collections.Generic;

namespace PBL3_MicayOnline.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public int? PromoCodeId { get; set; }

    public virtual ICollection<DeliveryLog> DeliveryLogs { get; set; } = new List<DeliveryLog>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual PromotionCode? PromoCode { get; set; }

    public virtual User? User { get; set; }
}
