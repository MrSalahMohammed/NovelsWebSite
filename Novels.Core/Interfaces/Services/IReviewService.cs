using Novels.Core.DTOs.Reader;

namespace Novels.Core.Interfaces.Services
{
    public interface IReviewService
    {
        Task<ReviewDto?> AddOrUpdateReviewAsync(int readerId, int novelId, ReviewRequest request);
        Task<bool> DeleteReviewAsync(int readerId, int novelId);
    }
}
