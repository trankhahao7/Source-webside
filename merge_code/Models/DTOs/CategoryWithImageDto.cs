namespace PBL3_MicayOnline.Models.DTOs
{
    public class CategoryWithImageDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? RepresentativeImage { get; set; } // URL ảnh đại diện

    }
}
