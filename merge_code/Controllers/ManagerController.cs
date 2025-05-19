using Microsoft.AspNetCore.Mvc;
using PBL3_MicayOnline.Services.Interfaces;

namespace PBL3_MicayOnline.Controllers
{
    public class ManagerController:Controller
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        public ManagerController(IUserService userService, IProductService productService)
        {
            _userService = userService;
            _productService = productService;
        }
        public async Task<IActionResult> Customer()
        {
            var customers = await _userService.GetAllUsersAsync();
            return View(customers);
        }
        public IActionResult AddCustomer()
        {
            return View();
        }
        public async Task<IActionResult> Product()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }
        public IActionResult AddProduct()
        {
            return View();
        }
        public IActionResult ManagerOrder()
        {
            return View();
        }
    }
}
