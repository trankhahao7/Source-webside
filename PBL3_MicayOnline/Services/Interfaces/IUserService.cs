using PBL3_MicayOnline.Models;

namespace PBL3_MicayOnline.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetTypedUserAsync(int id); // Gọi qua Factory
        Task<User?> LoginAsync(string username, string passwordHash);
        Task<bool> ChangePasswordAsync(int userId, string newPasswordHash);
    }
}
