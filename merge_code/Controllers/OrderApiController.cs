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

        public OrderApiController(IOrderService orderService)
        {
            _orderService = orderService;
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
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderCreateDto dto)
        {
            var created = await _orderService.CreateOrderAsync(dto);
            if (created == null) return BadRequest("Sản phẩm không hợp lệ");
            return CreatedAtAction(nameof(GetOrder), new { id = created.OrderId }, created);
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
    }
}
