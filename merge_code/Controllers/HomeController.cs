using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Services.Interfaces;

namespace PBL3_MicayOnline.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICategoryService _categoryService;

    public HomeController(ILogger<HomeController> logger, ICategoryService categoryService)
    {
        _logger = logger;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        // Lấy danh sách danh mục với ảnh đại diện
        var categories = await _categoryService.GetAllCategoriesWithRepresentativeImageAsync();
        return View(categories);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
