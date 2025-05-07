using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;

namespace PBL3_MicayOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackApiController : ControllerBase
    {
        private readonly Pbl3Context _context;

        public FeedbackApiController(Pbl3Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetAll()
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Product)
                .Select(f => new FeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    UserId = f.UserId ?? 0,
                    Username = f.User.Username,
                    ProductId = f.ProductId ?? 0,
                    ProductName = f.Product.Name,
                    Rating = (int)f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt ?? DateTime.Now,
                    IsApproved = f.IsApproved
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackDto>> GetById(int id)
        {
            var f = await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Product)
                .Where(f => f.FeedbackId == id)
                .Select(f => new FeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    UserId = f.UserId ?? 0,
                    Username = f.User.Username,
                    ProductId = f.ProductId ?? 0,
                    ProductName = f.Product.Name,
                    Rating = (int)f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt ?? DateTime.Now,
                    IsApproved = f.IsApproved
                })
                .FirstOrDefaultAsync();

            if (f == null) return NotFound();
            return f;
        }

        [HttpPost]
        public async Task<ActionResult<Feedback>> Create(FeedbackCreateDto dto)
        {
            var feedback = new Feedback
            {
                UserId = dto.UserId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.Now,
                IsApproved = false
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = feedback.FeedbackId }, feedback);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound();

            feedback.IsApproved = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound();

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
