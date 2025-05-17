using System;

namespace PBL3_MicayOnline.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string Status { get; set; } = null!;

        public DateTime? PaidAt { get; set; }

        public string? TransactionCode { get; set; }

        public int? ConfirmedBy { get; set; }

        public decimal? RefundedAmount { get; set; }

        public string PaymentChannel { get; set; } = null!;

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; } = null!;

        public virtual User? ConfirmedByUser { get; set; }
    }
}
