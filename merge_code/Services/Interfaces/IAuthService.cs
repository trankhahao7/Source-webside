using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
namespace PBL3_MicayOnline.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string username, string password);

        // Optional: Admin thay đổi mật khẩu người khác
        Task<bool> AdminForceChangePasswordAsync(int userId, string newPasswordHash);
    }
}

