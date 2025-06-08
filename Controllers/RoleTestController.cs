using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PBL3_MicayOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleTestController : ControllerBase
    {
        // Không cần đăng nhập
        [HttpGet("public")]
        public IActionResult Public() => Ok("✅ Public: Ai cũng truy cập được");

        // Bất kỳ user đã đăng nhập
        [Authorize]
        [HttpGet("auth")]
        public IActionResult Authenticated() =>
            Ok($"✅ Authenticated: Xin chào {User.Identity?.Name}");

        // Chỉ Admin
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnly() =>
            Ok($"✅ Admin: Xin chào {User.Identity?.Name}, bạn có quyền Admin");

        // Chỉ Customer
        [Authorize(Roles = "Customer")]
        [HttpGet("customer")]
        public IActionResult CustomerOnly() =>
            Ok($"✅ Customer: Xin chào {User.Identity?.Name}, bạn là khách hàng");

        // Chỉ Employee
        [Authorize(Roles = "Employee")]
        [HttpGet("employee")]
        public IActionResult EmployeeOnly() =>
            Ok($"✅ Employee: Xin chào {User.Identity?.Name}, bạn là nhân viên");
    }
}
