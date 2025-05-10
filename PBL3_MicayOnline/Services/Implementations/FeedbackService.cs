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

        public async Task<IEnumerable<FeedbackDto>> GetAllFeedbacksAsync()
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

        public async Task<FeedbackDto?> GetFeedbackByIdAsync(int id)
        {
            return await _context.Feedbacks
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
        }

        public async Task<FeedbackDto> CreateFeedbackAsync(FeedbackCreateDto dto)
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

            var user = await _context.Users.FindAsync(dto.UserId);
            var product = await _context.Products.FindAsync(dto.ProductId);

            return new FeedbackDto
            {
                FeedbackId = feedback.FeedbackId,
                UserId = feedback.UserId ?? 0,
                Username = user?.Username ?? "",
                ProductId = feedback.ProductId ?? 0,
                ProductName = product?.Name ?? "",
                Rating = feedback.Rating ?? 0,
                Comment = feedback.Comment,
                CreatedAt = feedback.CreatedAt ?? DateTime.Now,
                IsApproved = feedback.IsApproved
            };
        }

        public async Task<bool> ApproveFeedbackAsync(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return false;

            feedback.IsApproved = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFeedbackAsync(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return false;

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}