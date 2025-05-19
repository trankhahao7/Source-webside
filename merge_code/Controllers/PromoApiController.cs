using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class PromoApiController : ControllerBase
{
    private readonly IPromoCodeService _promoCodeService;

    public PromoApiController(IPromoCodeService promoCodeService)
    {
        _promoCodeService = promoCodeService;
    }

    // GET: api/promo
    [HttpGet]
    [Authorize] // Bất kỳ ai đăng nhập cũng xem được
    public async Task<ActionResult<IEnumerable<PromoCodeDto>>> GetAll()
    {
        var codes = await _promoCodeService.GetAllAsync();
        return Ok(codes);
    }

    // GET: api/promo/5
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<PromoCodeDto>> GetById(int id)
    {
        var promo = await _promoCodeService.GetByIdAsync(id);
        if (promo == null) return NotFound();
        return Ok(promo);
    }

    // POST: api/promo
    [HttpPost]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<ActionResult<PromoCodeDto>> Create([FromBody] PromoCodeCreateDto dto)
    {
        var created = await _promoCodeService.CreateAsync(dto);
        if (created == null) return BadRequest("Mã giảm giá không hợp lệ.");
        return CreatedAtAction(nameof(GetById), new { id = created.PromoCodeId }, created);
    }

    // PUT: api/promo/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Update(int id, [FromBody] PromoCodeCreateDto dto)
    {
        var updated = await _promoCodeService.UpdateAsync(id, dto);
        if (!updated) return NotFound();
        return NoContent();
    }

    // DELETE: api/promo/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Employee")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _promoCodeService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
