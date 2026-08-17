using Novels.Domain.Entities;

namespace Novels.Core.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        Task<Review?> GetReviewByUserAndNovelAsync(int readerId, int novelId);
        void AddReview(Review review);
        void RemoveReview(Review review);
        Task SaveChangesAsync();
    }
}
