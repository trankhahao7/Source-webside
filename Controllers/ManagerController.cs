using Microsoft.AspNetCore.Mvc;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Implementations;
using PBL3_MicayOnline.Services.Interfaces;

namespace PBL3_MicayOnline.Controllers
{
    public class ManagerController:Controller
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        public ManagerController(IUserService userService, IProductService productService, IOrderService orderService)
        {
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
        }

        // GET: /Manager/UpdateAccount/1005
        [HttpGet]
        [Route("Manager/UpdateAccount/{id}")]
        public async Task<IActionResult> UpdateAccount(int id)
        {
            var userUpdateDto = await _userService.GetUserForUpdateAsync(id);
            if (userUpdateDto == null)
                return NotFound();

            return View(userUpdateDto);
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

            var success = await _userService.UpdateUserAsync(id, dto);
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
        [HttpGet]
        public async Task<IActionResult> Statistic(DateTime? date, string type = "day")
        {
            var statisticViewModel = await _orderService.GetStatisticViewModelAsync(date ?? DateTime.Today, type);
            return View(statisticViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetStatisticData(DateTime? date, string type = "month")
        {
            try
            {
                var targetDate = date ?? DateTime.Today;
                var statisticData = await _orderService.GetRevenueStatisticAsync(targetDate, type);
                return Json(statisticData);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu muốn
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }


    }
}
