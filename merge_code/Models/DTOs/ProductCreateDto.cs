﻿namespace PBL3_MicayOnline.Models.DTOs
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsPopular { get; set; }
        public bool? IsActive { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
