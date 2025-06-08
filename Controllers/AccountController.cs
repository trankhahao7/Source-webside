using Microsoft.AspNetCore.Mvc;
using PBL3_MicayOnline.Services.Interfaces; 

public class AccountController : Controller
{
    private readonly ICartService _cartService;

    public AccountController(ICartService cartService)
    {
        _cartService = cartService;
    }

    public IActionResult Login() => View();
    public IActionResult Create() => View();
    public IActionResult ForgotPassword() => View();
    public IActionResult ResetPassword() => View();
    public IActionResult ChangePassword() => View();
    public IActionResult Setting() => View();
    public IActionResult Security() => View();

    [HttpGet("/logout")]
    public IActionResult Logout()
    {
        _cartService.ClearCart(HttpContext);


        return Redirect("/");
    }

    [HttpGet("/payment")]
    public IActionResult Payment()
    {
        _cartService.ClearCart(HttpContext);
        return Redirect("/Payment/SelectPayment");
    }
}
