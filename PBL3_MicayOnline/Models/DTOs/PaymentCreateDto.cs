namespace PBL3_MicayOnline.Models.DTOs
{
    public class PaymentCreateDto
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentChannel { get; set; }
        public string? Note { get; set; }
    }
}
