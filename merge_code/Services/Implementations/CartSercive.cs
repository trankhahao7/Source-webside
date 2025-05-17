using System.Security.Claims;
using PBL3_MicayOnline.Models.DTOs;

public class CartService : ICartService
{
    private string GetCartSessionKey(HttpContext httpContext)
    {
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
                return $"Cart_{userId}";
        }
        return "Cart_Guest";
    }

    public List<CartItemDto> GetCart(HttpContext httpContext)
    {
        var session = httpContext.Session;
        var key = GetCartSessionKey(httpContext);
        var cartJson = session.GetString(key);
        if (string.IsNullOrEmpty(cartJson))
            return new List<CartItemDto>();
        return System.Text.Json.JsonSerializer.Deserialize<List<CartItemDto>>(cartJson) ?? new List<CartItemDto>();
    }

    public void AddToCart(HttpContext httpContext, ProductDto product, int quantity)
    {
        var cart = GetCart(httpContext);
        var item = cart.FirstOrDefault(x => x.ProductId == product.ProductId);
        if (item != null)
        {
            item.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItemDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Quantity = quantity
            });
        }
        SaveCart(httpContext, cart);
    }

    public void UpdateQuantity(HttpContext httpContext, int productId, int quantity)
    {
        var cart = GetCart(httpContext);
        var item = cart.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            item.Quantity = quantity;
            SaveCart(httpContext, cart);
        }
    }

    public void RemoveFromCart(HttpContext httpContext, int productId)
    {
        var cart = GetCart(httpContext);
        cart.RemoveAll(x => x.ProductId == productId);
        SaveCart(httpContext, cart);
    }

    public void ClearCart(HttpContext httpContext)
    {
        var session = httpContext.Session;
        var key = GetCartSessionKey(httpContext);
        session.Remove(key);
    }

    private void SaveCart(HttpContext httpContext, List<CartItemDto> cart)
    {
        var session = httpContext.Session;
        var key = GetCartSessionKey(httpContext);
        session.SetString(key, System.Text.Json.JsonSerializer.Serialize(cart));
    }
}
