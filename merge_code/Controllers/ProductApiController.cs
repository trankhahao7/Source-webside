using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;

namespace PBL3_MicayOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductService _productService;
        
        public ProductApiController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/product
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }

        // GET: api/product/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: api/product
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromForm] ProductCreateDto dto)
        {
            var created = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetProduct), new { id = created.ProductId }, created);
        }

        // PUT: api/product/{id}
        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
        {
            if (id != dto.ProductId) return BadRequest();

            var success = await _productService.UpdateProductAsync(id, dto);
            if (!success) return NotFound();

            return NoContent();
        }

        // DELETE: api/product/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}