public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }  
    public DateTime? OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
    public int? PromoCodeId { get; set; }
}
