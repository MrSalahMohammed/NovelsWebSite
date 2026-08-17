using Novels.Domain.Entities;

namespace Novels.Core.Interfaces.Repositories
{
    public interface IAuthorRepository
    {
        Task<AuthorProfile?> GetAuthorProfileByUserIdAsync(int userId);
        void AddAuthorProfile(AuthorProfile authorProfile);
        Task SaveChangesAsync();
    }
}
