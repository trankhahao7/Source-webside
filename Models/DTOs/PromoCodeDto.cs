namespace PBL3_MicayOnline.Models.DTOs
{
    public class PromoCodeDto
    {
        public int PromoCodeId { get; set; }
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? MaxUsage { get; set; }
        public int? UsedCount { get; set; }
        public decimal MinOrderValue { get; set; }
    }
    public class PromoCodeCreateDto
    {
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int? MaxUsage { get; set; }
        public decimal MinOrderValue { get; set; }
    }
    public class PromoCodeCheckResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}
