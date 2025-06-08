using PBL3_MicayOnline.Models.DTOs;
namespace PBL3_MicayOnline.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentDto>> GetAllAsync();
        Task<PaymentDto?> GetByIdAsync(int id);
        Task<(bool Success, string Message, PaymentDto? Payment)> CreatePaymentForOrderAsync(PaymentCreateDto dto);


        Task<bool> UpdateStatusAsync(int paymentId, string status, int confirmedBy);

        Task<bool> RefundAsync(int id, decimal refundedAmount);
        Task<bool> DeleteAsync(int id);
    }
}