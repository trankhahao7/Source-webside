using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;

namespace PBL3_MicayOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderApiController : ControllerBase
    {
        private readonly Pbl3Context _context;

        public OrderApiController(Pbl3Context context)
        {
            _context = context;
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    Username = o.User.Username,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    OrderDate = o.OrderDate,
                    PromoCodeId = o.PromoCodeId
                })
                .ToListAsync();

            return orders;
        }

        // GET: api/order/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Where(o => o.OrderId == id)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    Username = o.User.Username,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    OrderDate = o.OrderDate,
                    PromoCodeId = o.PromoCodeId
                })
                .FirstOrDefaultAsync();

            if (order == null) return NotFound();
            return order;
        }

        // POST: api/order
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderCreateDto dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                PromoCodeId = dto.PromoCodeId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = 0,
                OrderDetails = new List<OrderDetail>()
            };

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest($"Product ID {item.ProductId} not found.");

                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = product.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });

                order.TotalAmount += product.Price * item.Quantity;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Trả về DTO sau khi tạo
            var result = new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                Username = (await _context.Users.FindAsync(order.UserId))?.Username ?? "",
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                PromoCodeId = order.PromoCodeId
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, result);
        }

        // PUT: api/order/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/order/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            _context.OrderDetails.RemoveRange(order.OrderDetails);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
