using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;
namespace PBL3_MicayOnline.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly Pbl3Context _context;
        private readonly IHashingService _hashingService;

        public UserService(Pbl3Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Orders)
                .Include(u => u.Feedbacks)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    Phone = u.Phone,
                    CreatedAt = u.CreatedAt,
                    OrderCount = u.Orders.Count,
                    FeedbackCount = u.Feedbacks.Count
                }).ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var u = await _context.Users
                .Include(u => u.Orders)
                .Include(u => u.Feedbacks)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (u == null) return null;

            return new UserDto
            {
                UserId = u.UserId,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                Phone = u.Phone,
                CreatedAt = u.CreatedAt,
                OrderCount = u.Orders.Count,
                FeedbackCount = u.Feedbacks.Count
            };
        }

        public async Task<UserDto> CreateUserAsync(UserCreateDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = _hashingService.HashPassword(dto.PasswordHash),
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = "Customer",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                OrderCount = 0,
                FeedbackCount = 0
            };
        }


        public async Task<bool> UpdateUserAsync(int id, UserUpdateDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.FullName = dto.FullName ?? user.FullName;
            user.Email = dto.Email ?? user.Email;
            user.Phone = dto.Phone ?? user.Phone;
            user.Role = dto.Role ?? user.Role;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetUserEntityAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPasswordHash)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.PasswordHash = newPasswordHash;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

    }
}