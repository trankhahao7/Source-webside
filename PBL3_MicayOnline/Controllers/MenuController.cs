using Microsoft.AspNetCore.Mvc;
using PBL3_MicayOnline.Services.Interfaces;
using PBL3_MicayOnline.Models.DTOs;
using System.Threading.Tasks;

namespace PBL3_MicayOnline.Controllers
{
    public class MenuController : Controller
    {
        private readonly ICategoryService _categoryService;

        public MenuController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Category()
        {
            var categories = await _categoryService.GetAllCategoriesWithRepresentativeImageAsync();
            return View(categories);
        }

        public async Task<IActionResult> Product(int categoryId)
        {
            var products = await _categoryService.GetProductsByCategoryIdAsync(categoryId);
            if (!products.Any())
            {
                return NotFound();
            }

            ViewData["CategoryName"] = products.First().CategoryName;
            return View(products);

        }

    }
}
