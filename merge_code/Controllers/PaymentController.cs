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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateOrderAndSelect()
    {
        var cart = _cartService.GetCart(HttpContext);
        if (cart == null || cart.Count == 0)
            return Json(new { success = false, message = "Giỏ hàng trống!" });

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var uid))
        {
            return Json(new { success = false, message = "Bạn cần đăng nhập để đặt hàng!" });
        }

        var order = new Order
        {
            OrderDate = DateTime.Now,
            Status = "Đang Xử Lý",
            TotalAmount = cart.Sum(x => x.Price * x.Quantity),
            UserId = uid,
            PromoCodeId = null,
        };
        _context.Orders.Add(order);
        _context.SaveChanges();

        foreach (var item in cart)
        {
            _context.OrderDetails.Add(new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Price
            });
        }
        _context.SaveChanges();

        _cartService.ClearCart(HttpContext);

        return Json(new { success = true });
    }

    [HttpGet]
    public IActionResult Select()
    {
        return View("SelectPayment");
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
