namespace PBL3_MicayOnline.Models.DTOs
{
    public class OrderCreateDto
    {
        public int UserId { get; set; }
        public int? PromoCodeId { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
