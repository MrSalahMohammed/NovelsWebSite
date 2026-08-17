using Microsoft.EntityFrameworkCore;
using Novels.Core.Interfaces.Repositories;
using Novels.Domain.Entities;
using Novels.Infrastructure.Data;

namespace Novels.Infrastructure.Repositories
{
    public class FavouritRepository : IFavouritRepository
    {
        private readonly AppDbContext _context;

        public FavouritRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsNovelInUserFavoritesAsync(int readerId, int novelId)
        {
            return await _context
                .Users.Where(u => u.Id == readerId)
                .AnyAsync(u => u.FavoriteNovels.Any(n => n.Id == novelId));
        }

        public void AddFavoriteAsync(ApplicationUser reader, Novel novel)
        {
            reader.FavoriteNovels.Add(novel);
            novel.NumOfCollections++;
        }

        public void RemoveFavoriteAsync(ApplicationUser reader, Novel novel)
        {
            reader.FavoriteNovels.Remove(novel);
            if (novel.NumOfCollections > 0)
                novel.NumOfCollections--;
        }

        public async Task<List<Novel>> GetFavoritesAsync(int readerId)
        {
            return await _context
                .Users.Where(u => u.Id == readerId)
                .SelectMany(u => u.FavoriteNovels)
                .Include(n => n.AuthorProfile)
                .Include(n => n.Tags)
                .Include(n => n.Categories)
                .Include(n => n.Chapters)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
