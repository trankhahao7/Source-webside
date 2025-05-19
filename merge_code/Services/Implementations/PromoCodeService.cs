using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;

namespace PBL3_MicayOnline.Services
{
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
    }
}
