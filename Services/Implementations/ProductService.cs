using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;
namespace PBL3_MicayOnline.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly Pbl3Context _context;
        private readonly IWebHostEnvironment _env;
        public ProductService(Pbl3Context context, IWebHostEnvironment env)
        {

            _context = context;
            _env = env;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    IsPopular = p.IsPopular,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var p = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (p == null) return null;

            return new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                IsPopular = p.IsPopular,
                IsActive = p.IsActive
            };
        }

        public async Task<ProductDto> CreateProductAsync(ProductCreateDto dto)
        {
            string imageUrl = null;
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }
                imageUrl = $"/images/{uniqueFileName}";
            }
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = imageUrl,
                CategoryId = dto.CategoryId,
                IsPopular = dto.IsPopular ?? false,
                IsActive = dto.IsActive ?? true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return new ProductDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = (await _context.Categories.FindAsync(product.CategoryId))?.Name,
                IsPopular = product.IsPopular,
                IsActive = product.IsActive
            };
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string? categoryName)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(categoryName) && categoryName != "ALL")
            {
                query = query.Where(p => p.Category != null && p.Category.Name == categoryName);
            }

            return await query.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : null,
                IsPopular = p.IsPopular,
                IsActive = p.IsActive
            }).ToListAsync();
        }

        public async Task<bool> UpdateProductAsync(int id, ProductUpdateDto dto)
        {
            if (id != dto.ProductId) return false;

            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;
            product.IsPopular = dto.IsPopular ?? product.IsPopular;
            product.IsActive = dto.IsActive ?? product.IsActive;

            // Xử lý cập nhật ảnh nếu có file mới
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }

                // Xoá file ảnh cũ nếu cần (tuỳ yêu cầu)
                // var oldFilePath = Path.Combine(_env.WebRootPath, product.ImageUrl.TrimStart('/'));
                // if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);

                product.ImageUrl = $"/images/{uniqueFileName}";
            }

            // Nếu không có ảnh mới ➜ GIỮ nguyên product.ImageUrl

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}