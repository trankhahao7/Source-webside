using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserCreateDto dto)
    {
        try
        {
            var createdUser = await _userService.CreateUserAsync(dto);
            return Ok(new { message = "Đăng ký thành công", userId = createdUser.UserId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _authService.LoginAsync(dto.Username, dto.PasswordHash);
        if (user == null) return Unauthorized("Invalid credentials");

        var token = _authService.GenerateJwtToken(user);

        return Ok(new
        {
            token,
            userId = user.UserId,
            username = user.Username,
            role = user.Role,
            email = user.Email
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var success = await _authService.AdminForceChangePasswordAsync(dto.UserId, dto.NewPasswordHash);
        if (!success) return NotFound("User not found");
        return Ok("Password updated successfully.");
    }
}
