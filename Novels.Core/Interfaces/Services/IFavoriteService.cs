using Novels.Core.DTOs.Novel;

namespace Novels.Core.Interfaces.Services
{
    public interface IFavoriteService
    {
        Task<bool> AddFavoriteAsync(int readerId, int novelId);
        Task<bool> RemoveFavoriteAsync(int readerId, int novelId);
        Task<List<NovelDto>> GetFavoritesAsync(int readerId);
    }
}
