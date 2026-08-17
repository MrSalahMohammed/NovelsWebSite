using Novels.Core.DTOs.Reader;

namespace Novels.Core.Interfaces.Services
{
    public interface IReadingProgressService
    {
        Task<ReadingProgressDto?> GetProgressAsync(int readerId, int novelId);
        Task<ReadingProgressDto?> UpdateProgressAsync(int readerId, int novelId, int chapterId);
    }
}
