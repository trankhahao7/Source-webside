using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PBL3_MicayOnline.Controllers
{
    public class AccountController: Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        public IActionResult Setting()
        {
            return View();
        }
        public IActionResult Security()
        {
            return View();
        }
        [HttpGet("/logout")]
        public IActionResult Logout()
        {
            // Xoá giỏ hàng khỏi Session
            var key = HttpContext.User.Identity?.IsAuthenticated == true
                ? $"Cart_{HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value}"
                : "Cart_Guest";
            HttpContext.Session.Remove(key);

            // (Tuỳ chọn) Xoá Auth Cookie nếu có

            return Redirect("/");
        }
        [HttpGet("/payment")]
        public IActionResult Payment()
        {
            // Xoá giỏ hàng khỏi Session
            var key = HttpContext.User.Identity?.IsAuthenticated == true
                ? $"Cart_{HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value}"
                : "Cart_Guest";
            HttpContext.Session.Remove(key);

            // (Tuỳ chọn) Xoá Auth Cookie nếu có

            return Redirect("/Payment/SelectPayment");
        }

    }
}
