namespace PBL3_MicayOnline.Models.DTOs
{
    public class PromoCodeCreateDto
    {
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int? MaxUsage { get; set; }
        public decimal MinOrderValue { get; set; }
    }
}
