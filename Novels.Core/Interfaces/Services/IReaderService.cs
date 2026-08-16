using Novels.Domain.Entities;

namespace Novels.Core.Interfaces.Services
{
    public interface IReaderService
    {
        Task<bool> AddFavoriteAsync(int readerId, int novelId);
        Task<bool> RemoveFavoriteAsync(int readerId, int novelId);
        Task<List<Novel>> GetFavoritesAsync(int readerId);

        Task<ReadingProgress?> GetProgressAsync(int readerId, int novelId);
        Task<ReadingProgress?> UpdateProgressAsync(int readerId, int novelId, int chapterId);

        Task<Review?> AddOrUpdateReviewAsync(int readerId, int novelId, int score, string? comment);
        Task<bool> DeleteReviewAsync(int readerId, int novelId);

        // ---------- Account management ----------
        Task<bool> UpdateReaderDataAsync(
            int readerId,
            string fName,
            string lName,
            string? recoveryEmail,
            string? phoneNumber
        );
        Task<bool> PromoteToAuthorAsync(int readerId, string? bio);
        Task<bool> DeleteReaderAsync(int readerId);
        Task<bool> ReactivateReaderAsync(int readerId);
        Task<List<ReadingProgress>> GetReadingHistoryAsync(int readerId);
    }
}
