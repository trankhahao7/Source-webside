using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;

namespace PBL3_MicayOnline.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<CreateOrderResult> CreateOrderAsync(OrderCreateDto dto, int currentUserId);
        Task<bool> UpdateOrderStatusAsync(int id, string status);
        Task<bool> DeleteOrderAsync(int id);
        Task<OrderDetailDto?> GetOrderDetailByIdAsync(int orderId);
        Task<List<OrderDto>> GetOrdersByTimeForStatistic(DateTime? orderDate, string type);

        Task<List<StatisticResultDto>> GetRevenueStatisticAsync(DateTime date, string type);

        Task<StatisticViewModel> GetStatisticViewModelAsync(DateTime date, string type);

        Task<IEnumerable<OrderWithPaymentDto>> GetOrdersWithPaymentsAsync();
    }
    public class StatisticResultDto
    {
        public int Time { get; set; }   // Day or Month
        public decimal Total { get; set; }
    }
    public class OrderWithPaymentDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public int? PromoCodeId { get; set; }
        public PaymentDto? Payment { get; set; }
    }

}
