using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;

namespace PBL3_MicayOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackApiController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackApiController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        // GET: api/feedback
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetAll()
        {
            var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
            return Ok(feedbacks);
        }

        // GET: api/feedback/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<FeedbackDto>> GetById(int id)
        {
            var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
            if (feedback == null) return NotFound();
            return Ok(feedback);
        }

        // POST: api/feedback
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<ActionResult<FeedbackDto>> Create(FeedbackCreateDto dto)
        {
            var created = await _feedbackService.CreateFeedbackAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.FeedbackId }, created);
        }

        // PUT: api/feedback/5/approve
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _feedbackService.ApproveFeedbackAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        // DELETE: api/feedback/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _feedbackService.DeleteFeedbackAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
