using Microsoft.AspNetCore.Mvc;
using PBL3_MicayOnline.Models.DTOs;
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

        // GET: /Manager/UpdateAccount/1005
        [HttpGet]
        [Route("Manager/UpdateAccount/{id}")]
        public async Task<IActionResult> UpdateAccount(int id)
        {
            var user = await _userService.GetUserByIdAsync(id); // giả sử bạn có hàm này
            if (user == null)
                return NotFound();

            var dto = new UserUpdateDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role
            };

            return View(dto); // Trả về View UpdateAccount.cshtml
        }

        // POST: /Manager/UpdateAccount/1005
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Manager/UpdateAccount/{id}")]
        public async Task<IActionResult> UpdateAccount(int id, UserUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var success = await _userService.UpdateUserAsync(id, dto); // cần có phương thức này
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Cập nhật thất bại.");
                return View(dto);
            }

            return RedirectToAction("Customer");
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
        public IActionResult Order()
        {
            return View();
        }
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

    }
}
