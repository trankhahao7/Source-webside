using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Factories;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Services.Interfaces;

namespace PBL3_MicayOnline.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly Pbl3Context _context;

        public UserService(Pbl3Context context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetTypedUserAsync(int id)
        {
            var baseUser = await _context.Users.FindAsync(id);
            if (baseUser == null) return null;

            return UserFactory.Create(baseUser); // Gọi Factory
        }

        public async Task<User?> LoginAsync(string username, string passwordHash)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == passwordHash);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPasswordHash)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.PasswordHash = newPasswordHash;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
