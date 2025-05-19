using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;

namespace PBL3_MicayOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentApiController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentApiController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // GET: api/payment
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments()
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(payments);
        }

        // GET: api/payment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPayment(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null) return NotFound();

            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Nếu không phải Admin và không sở hữu đơn => chặn
            if (role != "Admin")
            {
                // Giả sử PaymentDto không chứa UserId, bạn cần kiểm tra từ Order nếu mở rộng
                // Ở đây kiểm tra sơ bộ bằng payment.OrderId nếu muốn
                return Forbid("Bạn không được phép xem thanh toán này.");
            }

            return Ok(payment);
        }

        // POST: api/payment
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> CreatePayment(PaymentCreateDto dto)
        {
            var created = await _paymentService.CreateAsync(dto);
            if (created == null) return BadRequest("Invalid OrderId or ProductId");
            return CreatedAtAction(nameof(GetPayment), new { id = created.PaymentId }, created);
        }

        // PUT: api/payment/5/status
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var success = await _paymentService.UpdateStatusAsync(id, status);
            if (!success) return NotFound();
            return NoContent();
        }

        // PUT: api/payment/5/refund
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/refund")]
        public async Task<IActionResult> UpdateRefund(int id, [FromBody] decimal refundedAmount)
        {
            var success = await _paymentService.RefundAsync(id, refundedAmount);
            if (!success) return NotFound();
            return NoContent();
        }

        // DELETE: api/payment/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var success = await _paymentService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
