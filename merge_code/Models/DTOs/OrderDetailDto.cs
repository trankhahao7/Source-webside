namespace PBL3_MicayOnline.Models.DTOs
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public int? PromoCodeId { get; set; }

        public List<OrderItemDetailDto> Items { get; set; } = new();
    }
}
