namespace PBL3_MicayOnline.Models.DTOs
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? TransactionCode { get; set; }
        public int? ConfirmedBy { get; set; }
        public decimal? RefundedAmount { get; set; }
        public string PaymentChannel { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
