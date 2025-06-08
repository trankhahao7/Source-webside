using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;

namespace PBL3_MicayOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Áp dụng cho toàn controller
    public class OrderApiController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderApiController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var orders = await _orderService.GetAllOrdersAsync();

            if (role == "Admin" || role == "Employee")
                return Ok(orders);

            // Lọc đơn của chính người dùng
            var userOrders = orders.Where(o => o.UserId == userId);
            return Ok(userOrders);
        }

        // GET: api/order/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrder(int id)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var order = await _orderService.GetOrderDetailByIdAsync(id);
            if (order == null) return NotFound();

            if (role != "Admin" && role != "Employee" && order.UserId != userId)
                return Forbid("Bạn không được phép xem đơn hàng này");

            return Ok(order);
        }

        // POST: api/order
        [Authorize(Roles = "Customer")]
        [HttpPost("CreateOrderAsync")]
        public async Task<IActionResult> CreateOrderAsync([FromBody] OrderCreateDto request)
        {
            // Lấy userId từ token đăng nhập
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { success = false, message = "Không xác thực được người dùng." });

            int currentUserId = int.Parse(userIdClaim);

            // Gọi OrderService xử lý toàn bộ logic đặt hàng
            var result = await _orderService.CreateOrderAsync(request, currentUserId);

            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message });

            // Nếu thành công, xóa giỏ hàng
            _cartService.ClearCart(HttpContext);
            _cartService.ClearCartByKey(HttpContext, "Cart_Guest");
            await HttpContext.Session.CommitAsync();

            return Ok(new
            {
                success = true,
                orderId = result.OrderId,
                finalAmount = result.FinalAmount,
                discountPercent = result.DiscountPercent,
                discountAmount = result.DiscountAmount,
                message = result.Message
            });
        }


        // PUT: api/order/{id}/status
        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var success = await _orderService.UpdateOrderStatusAsync(id, status);
            if (!success) return NotFound();
            return NoContent();
        }

        // DELETE: api/order/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var success = await _orderService.DeleteOrderAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
        [HttpGet("statistic")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderStatistics([FromQuery] DateTime? date, [FromQuery] string type)
        {
            var result = await _orderService.GetOrdersByTimeForStatistic(date,type);
            return Ok(result);
        }

        // GET: api/OrderApi/WithPayments
        [HttpGet("WithPayments")]
        public async Task<ActionResult<IEnumerable<OrderWithPaymentDto>>> GetOrdersWithPayments()
        {
            var result = await _orderService.GetOrdersWithPaymentsAsync();
            return Ok(result);
        }


    }
}
