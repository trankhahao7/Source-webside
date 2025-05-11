using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;
namespace PBL3_MicayOnline.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly Pbl3Context _context;

        public CategoryService(Pbl3Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.Products.Count
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.CategoryId == id)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.Products.Count
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                ProductCount = 0
            };
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryCreateDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.Name = dto.Name;
            category.Description = dto.Description;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<CategoryWithImageDto>> GetAllCategoriesWithRepresentativeImageAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryWithImageDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    RepresentativeImage = _context.Products
                        .Where(p => p.CategoryId == c.CategoryId)
                        .Select(p => new ProductDto
                        {
                            ProductId = p.ProductId,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            ImageUrl = p.ImageUrl,
                            CategoryId = p.CategoryId,
                            CategoryName = c.Name,
                            IsPopular = p.IsPopular,
                            IsActive = p.IsActive
                        })
                        .FirstOrDefault() != null // Check for null explicitly
                        ? _context.Products
                            .Where(p => p.CategoryId == c.CategoryId)
                            .Select(p => p.ImageUrl)
                            .FirstOrDefault()
                        : null // Return null if no product is found
                })
                .ToListAsync();
        }



        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryIdAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId,
                    CategoryName = _context.Categories
                        .Where(c => c.CategoryId == p.CategoryId)
                        .Select(c => c.Name)
                        .FirstOrDefault(),
                    IsPopular = p.IsPopular,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

    }
}