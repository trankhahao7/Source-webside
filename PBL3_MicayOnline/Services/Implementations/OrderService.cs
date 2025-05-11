using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;
namespace PBL3_MicayOnline.Services.Implementations
{


    public class OrderService : IOrderService
    {
        private readonly Pbl3Context _context;

        public OrderService(Pbl3Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            return await _context.Orders
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
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
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
        }

        public async Task<OrderDto?> CreateOrderAsync(OrderCreateDto dto)
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
                if (product == null) return null;

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

            var username = (await _context.Users.FindAsync(order.UserId))?.Username ?? "";

            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                Username = username,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                PromoCodeId = order.PromoCodeId
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return false;

            _context.OrderDetails.RemoveRange(order.OrderDetails);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}