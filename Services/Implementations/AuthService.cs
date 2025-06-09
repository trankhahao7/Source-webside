using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PBL3_MicayOnline.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly Pbl3Context _context;
        private readonly IHashingService _hashingService;
        private readonly IConfiguration _configuration;

        public AuthService(
            Pbl3Context context,
            IHashingService hashingService,
            IConfiguration configuration)
        {
            _context = context;
            _hashingService = hashingService;
            _configuration = configuration;
        }

        // 1. Đăng nhập
        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            var isValid = _hashingService.VerifyPassword(password, user.PasswordHash);
            return isValid ? user : null;
        }

        // 2. Đổi mật khẩu cho chính mình (người dùng đang đăng nhập)
        public async Task<bool> ChangeOwnPasswordAsync(int userId, ChangePasswordRequestDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (!_hashingService.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                throw new InvalidOperationException("Mật khẩu hiện tại không đúng.");

            user.PasswordHash = _hashingService.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return true;
        }

        // 3. Admin đổi mật khẩu cho người khác
        public async Task<bool> AdminForceChangePasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.PasswordHash = _hashingService.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return true;
        }

        // 4. Sinh JWT token
        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JwtSettings:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
