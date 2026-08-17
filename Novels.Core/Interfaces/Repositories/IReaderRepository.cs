using Novels.Domain.Entities;

namespace Novels.Core.Interfaces.Repositories
{
    public interface IReaderRepository
    {
        Task<ApplicationUser?> GetReaderByID(int readerId);
        Task<bool> DeleteReader(ApplicationUser user);
        Task<bool> UpdateReaderDataAsync(ApplicationUser user);
        Task<bool> IsUserInRoleAsync(ApplicationUser user, string role);
        Task<bool> AddUserToRoleAsync(ApplicationUser user, string role);

        Task SaveChangesAsync();
        Task<List<ReadingProgress>> GetReadingHistoryAsync(int readerId);
    }
}
