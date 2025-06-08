namespace PBL3_MicayOnline.Models.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public int ProductCount { get; set; }
    }
    public class CategoryCreateDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
    public class CategoryWithImageDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? RepresentativeImage { get; set; } // URL ảnh đại diện

    }
}
