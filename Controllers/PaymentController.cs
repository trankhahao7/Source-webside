using Microsoft.AspNetCore.Mvc;
using PBL3_MicayOnline.Services.Interfaces;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using System.Security.Claims;

public class PaymentController : Controller
{
    private readonly ICartService _cartService;
    private readonly Pbl3Context _context;

    public PaymentController(ICartService cartService, Pbl3Context context)
    {
        _cartService = cartService;
        _context = context;
    }



    [HttpGet]
    public IActionResult SelectPayment(int orderId)
    {
        ViewBag.OrderId = orderId;
        return View();
    }


    public IActionResult SelectPayment()
    {
        return View();
    }

    public IActionResult PaymentByCOD()
    {
        return View();
    }
}
