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
    }
}
