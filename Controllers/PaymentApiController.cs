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

                return Forbid("Bạn không được phép xem thanh toán này.");
            }

            return Ok(payment);
        }

        // POST: api/payment
        [Authorize(Roles = "Customer")]
        [HttpPost("SelectPayment")]
        public async Task<IActionResult> SelectPayment([FromBody] PaymentCreateDto dto)
        {
            var (success, message, payment) = await _paymentService.CreatePaymentForOrderAsync(dto);
            if (!success)
                return NotFound(new { success = false, message });

            return Ok(new { success = true, data = payment });
        }

        // PUT: api/payment/5/status
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _paymentService.UpdateStatusAsync(id, status, adminId);
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
