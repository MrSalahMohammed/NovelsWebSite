using Novels.Core.DTOs.Reader;

namespace Novels.Core.Interfaces.Services
{
    public interface IReaderService
    {
        Task<bool> DeleteReaderAsync(int readerId);
        Task<bool> UpdateReaderDataAsync(int readerId, UpdateReaderRequest request);
        Task<List<ReadingProgressDto>> GetReadingHistoryAsync(int readerId);
    }
}
