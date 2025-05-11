using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Services.Interfaces;
namespace PBL3_MicayOnline.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly Pbl3Context _context;
        private readonly IHashingService _hashingService;

        public AuthService(Pbl3Context context, IHashingService hashingService)
        {
            _context = context;
            _hashingService = hashingService;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            var isValid = _hashingService.VerifyPassword(password, user.PasswordHash);
            return isValid ? user : null;
        }

        public async Task<bool> AdminForceChangePasswordAsync(int userId, string newPasswordHash)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.PasswordHash = newPasswordHash;
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
