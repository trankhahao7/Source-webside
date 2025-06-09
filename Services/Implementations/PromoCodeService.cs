using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;

namespace PBL3_MicayOnline.Services
{
    // DTO kết quả kiểm tra mã giảm giá
    public class PromoCodeCheckResult
    {
        public bool IsValid { get; set; }
        
        public string Message { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }

        public int? PromoCodeId { get; set; }
    }

    public class PromoCodeService : IPromoCodeService
    {
        private readonly Pbl3Context _context;

        public PromoCodeService(Pbl3Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PromoCodeDto>> GetAllAsync()
        {
            return await _context.PromotionCodes
                .Select(p => new PromoCodeDto
                {
                    PromoCodeId = p.PromoCodeId,
                    Code = p.Code,
                    Discount = p.Discount,
                    ExpiryDate = p.ExpiryDate,
                    MaxUsage = p.MaxUsage,
                    UsedCount = p.UsedCount,
                    MinOrderValue = p.MinOrderValue ?? 0
                })
                .ToListAsync();
        }

        public async Task<PromoCodeDto?> GetByIdAsync(int id)
        {
            var p = await _context.PromotionCodes.FindAsync(id);
            if (p == null) return null;

            return new PromoCodeDto
            {
                PromoCodeId = p.PromoCodeId,
                Code = p.Code,
                Discount = p.Discount,
                ExpiryDate = p.ExpiryDate,
                MaxUsage = p.MaxUsage,
                UsedCount = p.UsedCount,
                MinOrderValue = p.MinOrderValue ?? 0
            };
        }

        public async Task<PromoCodeDto?> CreateAsync(PromoCodeCreateDto dto)
        {
            var promo = new PromotionCode
            {
                Code = dto.Code,
                Discount = dto.Discount,
                ExpiryDate = dto.ExpiryDate,
                MaxUsage = dto.MaxUsage,
                MinOrderValue = dto.MinOrderValue,
                UsedCount = 0
            };

            _context.PromotionCodes.Add(promo);
            await _context.SaveChangesAsync();

            return new PromoCodeDto
            {
                PromoCodeId = promo.PromoCodeId,
                Code = promo.Code,
                Discount = promo.Discount,
                ExpiryDate = promo.ExpiryDate,
                MaxUsage = promo.MaxUsage,
                UsedCount = promo.UsedCount,
                MinOrderValue = promo.MinOrderValue ?? 0
            };
        }

        public async Task<bool> UpdateAsync(int id, PromoCodeCreateDto dto)
        {
            var promo = await _context.PromotionCodes.FindAsync(id);
            if (promo == null) return false;

            promo.Code = dto.Code;
            promo.Discount = dto.Discount;
            promo.ExpiryDate = dto.ExpiryDate;
            promo.MaxUsage = dto.MaxUsage;
            promo.MinOrderValue = dto.MinOrderValue;

            _context.PromotionCodes.Update(promo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var promo = await _context.PromotionCodes.FindAsync(id);
            if (promo == null) return false;

            _context.PromotionCodes.Remove(promo);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Kiểm tra và tính toán mã giảm giá cho đơn hàng.
        /// </summary>
        public async Task<PromoCodeCheckResult> CheckPromoCodeAsync(string code, decimal orderTotal)
        {
            var promo = await _context.PromotionCodes
                .FirstOrDefaultAsync(p => p.Code == code);

            if (promo == null)
            {
                return new PromoCodeCheckResult
                {
                    IsValid = false,
                    Message = "Mã giảm giá không tồn tại.",
                    DiscountPercent = 0,
                    DiscountAmount = 0,
                    FinalAmount = orderTotal
                };
            }

            if (promo.ExpiryDate < DateTime.Now)
            {
                return new PromoCodeCheckResult
                {
                    IsValid = false,
                    Message = "Mã giảm giá đã hết hạn.",
                    DiscountPercent = 0,
                    DiscountAmount = 0,
                    FinalAmount = orderTotal
                };
            }

            if (promo.MaxUsage.HasValue && promo.UsedCount.HasValue && promo.UsedCount.Value >= promo.MaxUsage.Value)
            {
                return new PromoCodeCheckResult
                {
                    IsValid = false,
                    Message = "Mã giảm giá đã đạt giới hạn lượt sử dụng.",
                    DiscountPercent = 0,
                    DiscountAmount = 0,
                    FinalAmount = orderTotal
                };
            }

            if (orderTotal < (promo.MinOrderValue ?? 0))
            {
                return new PromoCodeCheckResult
                {
                    IsValid = false,
                    Message = $"Đơn hàng chưa đạt giá trị tối thiểu để áp dụng mã giảm giá ({promo.MinOrderValue:N0}đ).",
                    DiscountPercent = 0,
                    DiscountAmount = 0,
                    FinalAmount = orderTotal
                };
            }

            // Hợp lệ
            var discountPercent = promo.Discount;
            var discountAmount = orderTotal * (discountPercent / 100);
            var finalAmount = orderTotal - discountAmount;

            return new PromoCodeCheckResult
            {
                IsValid = true,
                PromoCodeId = promo.PromoCodeId,
                Message = "Áp dụng mã giảm giá thành công.",
                DiscountPercent = discountPercent,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount
            };
        }

        /// <summary>
        /// Tăng UsedCount của mã giảm giá sau khi đơn hàng đặt thành công.
        /// </summary>
        public async Task<bool> IncreaseUsedCountAsync(string code)
        {
            var promo = await _context.PromotionCodes.FirstOrDefaultAsync(p => p.Code == code);
            if (promo == null) return false;

            if (promo.UsedCount.HasValue)
                promo.UsedCount++;
            else
                promo.UsedCount = 1;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<int?> GetPromoCodeIdByCodeAsync(string code)
        {
            var promo = await _context.PromotionCodes.FirstOrDefaultAsync(p => p.Code == code);
            return promo?.PromoCodeId;
        }

    }
}
