using PBL3_MicayOnline.Models.DTOs;

namespace PBL3_MicayOnline.Services.Interfaces
{
    public interface IPromoCodeService
    {
        Task<IEnumerable<PromoCodeDto>> GetAllAsync();
        Task<PromoCodeDto?> GetByIdAsync(int id);
        Task<PromoCodeDto?> CreateAsync(PromoCodeCreateDto dto);
        Task<bool> UpdateAsync(int id, PromoCodeCreateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<int?> GetPromoCodeIdByCodeAsync(string code);
        Task<PromoCodeCheckResult> CheckPromoCodeAsync(string code, decimal orderTotal);
        Task<bool> IncreaseUsedCountAsync(string code);
    }

}
