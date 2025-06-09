using Microsoft.AspNetCore.Mvc;
using PBL3_MicayOnline.Services.Interfaces;
using PBL3_MicayOnline.Models.DTOs;
using System.Security.Claims;
using PBL3_MicayOnline.Models; // Thêm using cho model
using PBL3_MicayOnline.Data;   // Thêm using cho DbContext
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Data;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using PBL3_MicayOnline.Services.Implementations;

public class OrdersController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;

    public OrdersController(IProductService productService, ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
    }
    [AllowAnonymous]
    public IActionResult Cart()
    {
        var cart = _cartService.GetCart(HttpContext);
        return View(cart);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> AddToCartAjax([FromBody] CartAddRequest req)
    {
        var product = await _productService.GetProductByIdAsync(req.ProductId);
        if (product == null)
            return Json(new { success = false, message = "Sản phẩm không tồn tại!" });

        _cartService.AddToCart(HttpContext, product, req.Quantity);
        return Json(new { success = true, message = "Thêm vào giỏ hàng thành công!" });
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult UpdateQuantityAjax([FromBody] UpdateQuantityRequest req)
    {
        _cartService.UpdateQuantity(HttpContext, req.ProductId, req.Quantity);
        var cart = _cartService.GetCart(HttpContext);
        var item = cart.FirstOrDefault(x => x.ProductId == req.ProductId);
        var itemTotal = item != null ? (item.Price * item.Quantity).ToString("N0") : "0";
        var newTotal = cart.Sum(x => x.Price * x.Quantity).ToString("N0");
        return Json(new { success = true, itemTotal, newTotal });
    }

    [HttpPost]
    [AllowAnonymous] // hoặc [Authorize] nếu bạn muốn chỉ cho người đăng nhập xóa
    [Route("/Orders/RemoveFromCartAjax")]
    public IActionResult RemoveFromCartAjax([FromBody] RemoveFromCartRequest req)
    {
        try
        {
            _cartService.RemoveFromCart(HttpContext, req.ProductId);
            return Json(new { success = true, message = "Đã xóa sản phẩm khỏi giỏ hàng." });
        }
        catch
        {
            return Json(new { success = false, message = "Xóa sản phẩm thất bại!" });
        }
    }

    public class RemoveFromCartRequest
    {
        public int ProductId { get; set; }
    }

    [HttpPost]
    [Authorize]
    public IActionResult RemoveFromCart(int productId)
    {
        _cartService.RemoveFromCart(HttpContext, productId);
        return RedirectToAction("Cart");
    }


    public class CartAddRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class UpdateQuantityRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public async Task<IActionResult> Order(string? category)
    {
        var products = string.IsNullOrEmpty(category) || category == "ALL"
            ? await _productService.GetAllProductsAsync()
            : await _productService.GetProductsByCategoryAsync(category);

        ViewBag.SelectedCategory = category ?? "ALL";
        return View(products.ToList());
    }

    [HttpPost]
    [Authorize] // đảm bảo chỉ người đăng nhập mới gọi được
    [Route("/Orders/ClearCartAjax")]
    public IActionResult ClearCartAjax()
    {
        try
        {
            _cartService.ClearCart(HttpContext);
            return Json(new { success = true });
        }
        catch
        {
            return Json(new { success = false, message = "Không thể xóa giỏ hàng." });
        }
    }

    [AllowAnonymous]
    public IActionResult OrderHistory()
    {
        return View();
    }




    // test mã giảm giá
    //[HttpGet]
    //[AllowAnonymous]
    //public IActionResult ValidatePromo(string code)
    //{
    //    var promo = _context.PromotionCodes.FirstOrDefault(p => p.Code == code);
    //    if (promo == null || promo.UsedCount >= promo.MaxUsage || promo.ExpiryDate < DateTime.Now)
    //        return Json(new { valid = false });

    //    var cart = _cartService.GetCart(HttpContext);
    //    var total = cart.Sum(item => item.Price * item.Quantity);
    //    if (total < promo.MinOrderValue)
    //        return Json(new { valid = false });

    //    return Json(new { valid = true, discount = promo.Discount, promoCodeId = promo.PromoCodeId });
    //}

}
