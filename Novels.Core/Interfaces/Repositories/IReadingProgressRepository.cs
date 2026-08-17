using Novels.Domain.Entities;

namespace Novels.Core.Interfaces.Repositories
{
    public interface IReadingProgressRepository
    {
        void AddProgress(ReadingProgress progress);
        Task<ReadingProgress?> GetProgressAsync(int readerId, int novelId);
        Task SaveChangesAsync();
    }
}
