using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;

namespace PBL3_MicayOnline.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackDto>> GetAllAsync();
        Task<FeedbackDto?> GetByIdAsync(int id);
        Task<Feedback> CreateAsync(Feedback feedback);
        Task<bool> ApproveAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
