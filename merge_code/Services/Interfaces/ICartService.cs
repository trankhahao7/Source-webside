using Microsoft.AspNetCore.Http;
using PBL3_MicayOnline.Models.DTOs;
using System.Collections.Generic;

public interface ICartService
{
    List<CartItemDto> GetCart(HttpContext httpContext);
    void AddToCart(HttpContext httpContext, ProductDto product, int quantity);
    void UpdateQuantity(HttpContext httpContext, int productId, int quantity);
    void RemoveFromCart(HttpContext httpContext, int productId);
    void ClearCart(HttpContext httpContext);
}
