using System;
using System.Collections.Generic;

namespace PBL3_MicayOnline.Models;

public partial class DeliveryLog
{
    public int DeliveryId { get; set; }

    public int? OrderId { get; set; }

    public string? ShipperName { get; set; }

    public DateTime? DeliveryTime { get; set; }

    public string? Notes { get; set; }

    public virtual Order? Order { get; set; }
}
