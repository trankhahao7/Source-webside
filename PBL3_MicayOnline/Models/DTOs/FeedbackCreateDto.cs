namespace PBL3_MicayOnline.Models.DTOs
{
    public class FeedbackCreateDto
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
