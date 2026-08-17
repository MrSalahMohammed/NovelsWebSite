using Novels.Domain.Entities;

namespace Novels.Core.Interfaces.Repositories
{
    public interface IFavouritRepository
    {
        void AddFavoriteAsync(ApplicationUser reader, Novel novel);
        void RemoveFavoriteAsync(ApplicationUser reader, Novel novel);
        Task<List<Novel>> GetFavoritesAsync(int readerId);
        Task<bool> IsNovelInUserFavoritesAsync(int readerId, int novelId);
        Task SaveChangesAsync();
    }
}
