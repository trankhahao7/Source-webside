using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;

namespace PBL3_MicayOnline.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string username, string password);
        Task<bool> ChangeOwnPasswordAsync(int userId, ChangePasswordRequestDto dto);
        Task<bool> AdminForceChangePasswordAsync(int userId, string newPassword);
        string GenerateJwtToken(User user);
    }
}
