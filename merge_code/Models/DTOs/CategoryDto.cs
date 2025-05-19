namespace PBL3_MicayOnline.Models.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public int ProductCount { get; set; }
    }
}
