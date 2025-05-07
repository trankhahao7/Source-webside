using Microsoft.EntityFrameworkCore;
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
using PBL3_MicayOnline.Services.Interfaces;

namespace PBL3_MicayOnline.Services.Implementations
{
    public class FeedbackService : IFeedbackService
    {
        private readonly Pbl3Context _context;

        public FeedbackService(Pbl3Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FeedbackDto>> GetAllAsync()
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Product)
                .Select(f => new FeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    UserId = (int)f.UserId,
                    Username = f.User.Username,
                    ProductId = (int)f.ProductId,
                    ProductName = f.Product.Name,
                    Rating = (int)f.Rating,
                    Comment = f.Comment,
                    CreatedAt = (DateTime)f.CreatedAt,
                    IsApproved = f.IsApproved ?? false
                })
                .ToListAsync();
        }

        public async Task<FeedbackDto?> GetByIdAsync(int id)
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Product)
                .Where(f => f.FeedbackId == id)
                .Select(f => new FeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    UserId = (int)f.UserId,
                    Username = f.User.Username,
                    ProductId = (int)f.ProductId,
                    ProductName = f.Product.Name,
                    Rating = (int)f.Rating,
                    Comment = f.Comment,
                    CreatedAt = (DateTime)f.CreatedAt,
                    IsApproved = f.IsApproved ?? false
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Feedback> CreateAsync(Feedback feedback)
        {
            feedback.CreatedAt = DateTime.Now;
            feedback.IsApproved = false;

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return feedback;
        }

        public async Task<bool> ApproveAsync(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return false;

            feedback.IsApproved = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return false;

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
