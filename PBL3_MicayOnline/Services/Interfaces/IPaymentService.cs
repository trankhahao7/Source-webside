using PBL3_MicayOnline.Models.DTOs;
namespace PBL3_MicayOnline.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync();
        Task<PaymentDto?> GetByIdAsync(int id);
        Task<PaymentDto?> CreateAsync(PaymentCreateDto dto);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> RefundAsync(int id, decimal refundedAmount);
        Task<bool> DeleteAsync(int id);
    }
}