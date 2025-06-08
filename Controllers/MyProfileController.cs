using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;
using PBL3_MicayOnline.Services.Implementations;

namespace PBL3_MicayOnline.Controllers
{
    [ApiController]
    [Route("api/my-profile")]
    [Authorize] // ✅ Cho mọi user đã đăng nhập
    public class MyProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IHashingService _hashingService;

        public MyProfileController(IUserService userService, IHashingService hashingService, IAuthService authService)
        {
            _userService = userService;
            _hashingService = hashingService;
            _authService = authService;
        }

        // GET: api/my-profile
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetOwnProfile()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound("Không tìm thấy người dùng.");
            return Ok(user);
        }

        // PUT: api/my-profile
        [HttpPut]
        public async Task<IActionResult> UpdateOwnProfile(UserUpdateDto dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _userService.UpdateUserAsync(userId, dto);
            if (!success) return NotFound("Không thể cập nhật thông tin.");
            return NoContent();
        }

        // PUT: api/my-profile/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangeOwnPassword(ChangePasswordRequestDto dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var updated = await _authService.ChangeOwnPasswordAsync(userId, dto);

                if (!updated) return NotFound("Không tìm thấy người dùng.");
                return Ok("Đổi mật khẩu thành công.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "Cập nhật mật khẩu thất bại.");
            }
        }

    }
}
