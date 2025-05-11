using PBL3_MicayOnline.Models.DTOs;
namespace PBL3_MicayOnline.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto?> CreateOrderAsync(OrderCreateDto dto);
        Task<bool> UpdateOrderStatusAsync(int id, string status);
        Task<bool> DeleteOrderAsync(int id);
    }
}