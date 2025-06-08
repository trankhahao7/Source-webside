using PBL3_MicayOnline.Models.DTOs;
namespace PBL3_MicayOnline.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackDto>> GetAllFeedbacksAsync();
        Task<FeedbackDto?> GetFeedbackByIdAsync(int id);
        Task<FeedbackDto> CreateFeedbackAsync(FeedbackCreateDto dto);
        Task<bool> ApproveFeedbackAsync(int id);
        Task<bool> DeleteFeedbackAsync(int id);
    }
}