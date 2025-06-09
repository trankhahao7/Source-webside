using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;
using System.Security.Claims;
namespace PBL3_MicayOnline.Services.Implementations
{


    public class OrderService : IOrderService
    {
        private readonly Pbl3Context _context;

        private readonly IPromoCodeService _promoCodeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderService(Pbl3Context context, IPromoCodeService promoCodeService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _promoCodeService = promoCodeService;
            _httpContextAccessor = httpContextAccessor;
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

        /// <summary>
        /// Tạo mới đơn hàng và xử lý mã giảm giá.
        /// </summary>
        public async Task<CreateOrderResult> CreateOrderAsync(OrderCreateDto dto, int currentUserId)
        {
            // 1. Validate đầu vào
            if (dto.Items == null || !dto.Items.Any())
                return new CreateOrderResult { Success = false, Message = "Không có sản phẩm nào trong đơn hàng." };

            if (dto.UserId != currentUserId)
                return new CreateOrderResult { Success = false, Message = "Không xác thực được người dùng." };

            // 2. Kiểm tra sản phẩm và tính tổng tiền
            decimal totalAmount = 0;
            var orderDetails = new List<OrderDetail>();
            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return new CreateOrderResult { Success = false, Message = $"Sản phẩm với ID {item.ProductId} không tồn tại." };

                orderDetails.Add(new OrderDetail
                {
                    ProductId = product.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });

                totalAmount += product.Price * item.Quantity;
            }

            // 3. Kiểm tra và áp dụng mã giảm giá nếu có
            decimal discountPercent = 0;
            decimal discountAmount = 0;
            decimal finalAmount = totalAmount;
            int? promoCodeId = dto.PromoCodeId;

            if (promoCodeId.HasValue)
            {
                // Lấy code từ promoCodeId
                var code = await _promoCodeService.GetByIdAsync(promoCodeId.Value);

                if (code == null)
                    return new CreateOrderResult { Success = false, Message = "Mã giảm giá không tồn tại." };

                var promoResult = await _promoCodeService.CheckPromoCodeAsync(code.Code, totalAmount);
                if (!promoResult.IsValid)
                    return new CreateOrderResult { Success = false, Message = promoResult.Message };

                discountPercent = promoResult.DiscountPercent;
                discountAmount = promoResult.DiscountAmount;
                finalAmount = promoResult.FinalAmount;
            }

            // 4. Tạo đơn hàng
            var order = new Order
            {
                UserId = dto.UserId,
                PromoCodeId = promoCodeId,
                OrderDate = DateTime.Now,
                Status = "Đang Xử Lý",
                TotalAmount = finalAmount,
                OrderDetails = orderDetails
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 5. Nếu có mã giảm giá, tăng UsedCount
            if (promoCodeId.HasValue)
            {
                var code = await _promoCodeService.GetByIdAsync(promoCodeId.Value);

                if (code != null)
                    await _promoCodeService.IncreaseUsedCountAsync(code.Code);
            }

            // 6. Trả về kết quả chi tiết
            return new CreateOrderResult
            {
                Success = true,
                Message = "Đặt hàng thành công.",
                OrderId = order.OrderId,
                FinalAmount = finalAmount,
                DiscountPercent = discountPercent,
                DiscountAmount = discountAmount
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
        public async Task<OrderDetailDto?> GetOrderDetailByIdAsync(int orderId)
{
    var order = await _context.Orders
        .Include(o => o.User)
        .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
        .FirstOrDefaultAsync(o => o.OrderId == orderId);

    if (order == null) return null;

    return new OrderDetailDto
    {
        OrderId = order.OrderId,
        UserId = order.UserId,
        Username = order.User?.Username ?? "",
        OrderDate = order.OrderDate,
        TotalAmount = order.TotalAmount,
        Status = order.Status,
        PromoCodeId = order.PromoCodeId,
        Items = order.OrderDetails.Select(od => new OrderItemDetailDto
        {
            ProductId = od.ProductId,
            ProductName = od.Product?.Name ?? "Không rõ",
            Quantity = od.Quantity,
            UnitPrice = od.UnitPrice
        }).ToList()
    };
}
        public async Task<List<OrderDto>> GetOrdersByTimeForStatistic(DateTime? orderDate, string type)
        {
            if (orderDate == null)
                return new List<OrderDto>();

            IQueryable<Order> query = _context.Orders.Include(o => o.User);

            switch (type.ToLower())
            {
                case "month":
                    query = query.Where(o => o.OrderDate.HasValue &&
                                             o.OrderDate.Value.Month == orderDate.Value.Month &&
                                             o.OrderDate.Value.Year == orderDate.Value.Year);
                    break;
                case "year":
                    query = query.Where(o => o.OrderDate.HasValue &&
                                             o.OrderDate.Value.Year == orderDate.Value.Year);
                    break;
                default:
                    return new List<OrderDto>();
            }

            var orders = await query.OrderBy(o => o.OrderDate).ToListAsync();

            return orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount
            }).ToList();
        }

        public async Task<List<StatisticResultDto>> GetRevenueStatisticAsync(DateTime date, string type)
        {
            if (type == "month")
            {
                var startDate = new DateTime(date.Year, date.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var orders = await _context.Orders
                    .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date >= startDate && o.OrderDate.Value.Date <= endDate)
                    .ToListAsync();

                var grouped = orders
                    .GroupBy(o => o.OrderDate.Value.Date)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.TotalAmount));

                var fullDays = Enumerable.Range(0, (endDate - startDate).Days + 1)
                    .Select(offset => startDate.AddDays(offset))
                    .Select(date => new StatisticResultDto
                    {
                        Time = date.Day,
                        Total = grouped.ContainsKey(date) ? grouped[date] : 0m
                    }).ToList();

                return fullDays;
            }
            else if (type == "year")
            {
                var startDate = new DateTime(date.Year, 1, 1);
                var endDate = startDate.AddYears(1).AddDays(-1);

                var orders = await _context.Orders
                    .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date >= startDate && o.OrderDate.Value.Date <= endDate)
                    .ToListAsync();

                var grouped = orders
                    .GroupBy(o => o.OrderDate.Value.Month)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.TotalAmount));

                var fullMonths = Enumerable.Range(1, 12)
                    .Select(month => new StatisticResultDto
                    {
                        Time = month,
                        Total = grouped.ContainsKey(month) ? grouped[month] : 0m
                    }).ToList();

                return fullMonths;
            }
            else
            {
                throw new ArgumentException("Chỉ hỗ trợ thống kê theo tháng hoặc năm.");
            }
        }


        public async Task<IEnumerable<OrderWithPaymentDto>> GetOrdersWithPaymentsAsync()
        {
            // Lấy thông tin người dùng từ token
            var user = _httpContextAccessor.HttpContext?.User;
            var role = user?.FindFirst(ClaimTypes.Role)?.Value;
            var userId = int.Parse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Query join Orders + Users, và lấy Payment
            var query = from o in _context.Orders
                        join u in _context.Users on o.UserId equals u.UserId
                        select new OrderWithPaymentDto
                        {
                            OrderId = o.OrderId,
                            UserId = o.UserId,
                            Username = u.Username,  // Lấy từ bảng Users
                            OrderDate = o.OrderDate,
                            TotalAmount = o.TotalAmount,
                            Status = o.Status,
                            PromoCodeId = o.PromoCodeId,
                            Payment = _context.Payments
                                .Where(p => p.OrderId == o.OrderId)
                                .Select(p => new PaymentDto
                                {
                                    PaymentId = p.PaymentId,
                                    OrderId = p.OrderId,
                                    Amount = p.Amount,
                                    PaymentMethod = p.PaymentMethod,
                                    Status = p.Status,
                                    PaidAt = p.PaidAt,
                                    TransactionCode = p.TransactionCode,
                                    ConfirmedBy = p.ConfirmedBy,
                                    RefundedAmount = p.RefundedAmount,
                                    PaymentChannel = p.PaymentChannel,
                                    Note = p.Note,
                                    CreatedAt = p.CreatedAt
                                })
                                .FirstOrDefault()
                        };

            // Nếu không phải admin, chỉ lấy order của chính user
            if (role != "Admin")
            {
                query = query.Where(o => o.UserId == userId);
            }

            return await query.ToListAsync();
        }

        public async Task<StatisticViewModel> GetStatisticViewModelAsync(DateTime date, string type)
        {
            var orders = await GetOrdersByTimeForStatistic(date, type);
            var totalRevenue = orders.Sum(o => o.TotalAmount);

            return new StatisticViewModel
            {
                Orders = orders,
                Date = date,
                Type = type,
                TotalRevenue = totalRevenue
            };
        }




    }
}