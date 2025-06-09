using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;
namespace PBL3_MicayOnline.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly Pbl3Context _context;

        public PaymentService(Pbl3Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            return await _context.Payments
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
                .ToListAsync();
        }

        public async Task<PaymentDto?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Where(p => p.PaymentId == id)
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
                .FirstOrDefaultAsync();
        }






        public async Task<(bool Success, string Message, PaymentDto? Payment)> CreatePaymentForOrderAsync(PaymentCreateDto dto)
        {
            var order = await _context.Orders.FindAsync(dto.OrderId);
            if (order == null)
                return (false, "Đơn hàng không tồn tại!", null);

            dto.Amount = order.TotalAmount; // Lấy amount từ DB, không dùng FE

            var payment = new Payment
            {
                OrderId = dto.OrderId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                PaymentChannel = dto.PaymentChannel,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                Note = dto.Note
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var paymentDto = new PaymentDto
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                PaidAt = payment.PaidAt,
                TransactionCode = payment.TransactionCode,
                ConfirmedBy = payment.ConfirmedBy,
                RefundedAmount = payment.RefundedAmount,
                PaymentChannel = payment.PaymentChannel,
                Note = payment.Note,
                CreatedAt = payment.CreatedAt
            };

            return (true, "Tạo thanh toán thành công", paymentDto);
        }


        public async Task<bool> UpdateStatusAsync(int paymentId, string status, int confirmedBy)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return false;

            payment.Status = status;
            payment.ConfirmedBy = confirmedBy; // Ghi id admin
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> RefundAsync(int id, decimal refundedAmount)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return false;

            payment.RefundedAmount = refundedAmount;
            payment.Status = "Refunded";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
