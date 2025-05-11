using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PBL3_MicayOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService; // nếu muốn dùng cho đăng ký
        private readonly IConfiguration _configuration;
        private readonly IHashingService _hashingService;

        public AuthController(IAuthService authService, IUserService userService, IConfiguration configuration, IHashingService hashingService)
        {
            _authService = authService;
            _userService = userService;
            _configuration = configuration;
            _hashingService = hashingService;
        }

        // POST: api/auth/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDto dto)
        {
            // 1. Kiểm tra username đã tồn tại
            var existing = await _userService.GetUserByUsernameAsync(dto.Username);
            if (existing != null)
                return BadRequest("Username đã tồn tại.");

            // 2. Hash mật khẩu
            var hashed = _hashingService.HashPassword(dto.PasswordHash);
            dto.PasswordHash = hashed;

            // 3. Tạo mới
            var createdUser = await _userService.CreateUserAsync(dto);
            return Ok(new { message = "Đăng ký thành công", userId = createdUser.UserId });
        }


        // POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _authService.LoginAsync(dto.Username, dto.PasswordHash);
            if (user == null) return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                userId = user.UserId,
                username = user.Username,
                role = user.Role,
                email = user.Email
            });
        }

        // PUT: api/auth/change-password
        [Authorize(Roles = "Admin")]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var newHashed = _hashingService.HashPassword(dto.NewPasswordHash);
            var success = await _authService.AdminForceChangePasswordAsync(dto.UserId, newHashed);
            if (!success) return NotFound("User not found");
            return Ok("Password updated successfully.");
        }

        // 🛠 Generate JWT Token
        private string GenerateJwtToken(User user)
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
