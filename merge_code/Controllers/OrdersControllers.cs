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

public class OrdersController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly Pbl3Context _context;

    public OrdersController(IProductService productService, ICartService cartService, Pbl3Context context)
    {
        _productService = productService;
        _cartService = cartService;
        _context = context;
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

    // === Thêm action này ===

    [HttpPost]
    [Authorize]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PlaceOrder([FromBody] OrderCreateDto request)
    {
        // 1. Kiểm tra tính hợp lệ
        if (request.Items == null || !request.Items.Any())
            return BadRequest(new { success = false, message = "Không có sản phẩm nào trong đơn hàng." });

        // 2. Kiểm tra userId gửi lên có đúng với user đang đăng nhập không (bảo mật)
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || int.Parse(userIdClaim) != request.UserId)
            return Unauthorized(new { success = false, message = "Không xác thực được người dùng." });

        // 3. Tạo đơn hàng
        var order = new Order
        {
            UserId = request.UserId,
            PromoCodeId = request.PromoCodeId,
            OrderDate = DateTime.Now,
            Status = "Đang Xử Lý"
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(); // Nhận OrderId

        decimal totalAmount = 0;

        // 4. Lặp tạo chi tiết đơn hàng từ danh sách sản phẩm
        var orderDetails = new List<OrderDetail>();

        foreach (var item in request.Items)
        {
            var product = await _productService.GetProductByIdAsync(item.ProductId);
            if (product == null)
                return BadRequest(new { success = false, message = $"Sản phẩm với ID {item.ProductId} không tồn tại." });

            var orderDetail = new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = product.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };

            totalAmount += product.Price * item.Quantity;
            orderDetails.Add(orderDetail);
        }

        _context.OrderDetails.AddRange(orderDetails);

        // 5. Cập nhật tổng tiền đơn hàng

        // Áp dụng mã giảm giá nếu có
        if (request.PromoCodeId.HasValue)
        {
            var promo = await _context.PromotionCodes.FindAsync(request.PromoCodeId.Value);
            if (promo != null && promo.UsedCount < promo.MaxUsage && promo.ExpiryDate > DateTime.Now)
            {
                if (totalAmount >= promo.MinOrderValue)
                {
                    totalAmount = totalAmount * (100 - promo.Discount) / 100;
                    promo.UsedCount += 1;
                }
            }
        }
        order.TotalAmount = totalAmount;
        await _context.SaveChangesAsync();

        // 6. Xóa giỏ hàng sau khi đặt
        _cartService.ClearCart(HttpContext);
        await HttpContext.Session.CommitAsync();

        return Ok(new { success = true, orderId = order.OrderId });
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
    [HttpGet]
    [AllowAnonymous]
    public IActionResult ValidatePromo(string code)
    {
        var promo = _context.PromotionCodes.FirstOrDefault(p => p.Code == code);
        if (promo == null || promo.UsedCount >= promo.MaxUsage || promo.ExpiryDate < DateTime.Now)
            return Json(new { valid = false });

        var cart = _cartService.GetCart(HttpContext);
        var total = cart.Sum(item => item.Price * item.Quantity);
        if (total < promo.MinOrderValue)
            return Json(new { valid = false });

        return Json(new { valid = true, discount = promo.Discount, promoCodeId = promo.PromoCodeId });
    }

}
