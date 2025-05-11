using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
namespace PBL3_MicayOnline.Services.Interfaces
{
    public interface IUserService
    {
        // Quản lý người dùng (dành cho Admin)
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(UserCreateDto dto);
        Task<bool> UpdateUserAsync(int id, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(int id);

        // Dành cho người dùng tự thao tác
        Task<User?> GetUserEntityAsync(int id);
        Task<bool> ChangePasswordAsync(int userId, string newPasswordHash);

        Task<User?> GetUserByUsernameAsync(string username);

    }

}