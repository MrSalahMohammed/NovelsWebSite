using Microsoft.EntityFrameworkCore;
using Novels.Core.Interfaces.Repositories;
using Novels.Domain.Entities;
using Novels.Infrastructure.Data;

namespace Novels.Infrastructure.Repositories
{
    public class NovelRepository : INovelRepository
    {
        private readonly AppDbContext _context;

        public NovelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Novel?> FindNovelByID(int novelId)
        {
            return await _context.Novels.FindAsync(novelId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ChapterBelongsToNovelAsync(int chapterId, int novelId)
        {
            return await _context.Chapters.AnyAsync(c => c.Id == chapterId && c.NovelId == novelId);
        }

        public async Task<bool> NovelExistsAsync(int novelId)
        {
            return await _context.Novels.AnyAsync(n => n.Id == novelId);
        }

        public async Task RecalculateNovelRatingAsync(int novelId)
        {
            var novel = await _context.Novels.FindAsync(novelId);
            if (novel is null)
                return;

            var scores = await _context
                .Reviews.Where(r => r.NovelId == novelId)
                .Select(r => r.Score)
                .ToListAsync();

            novel.RatingsCount = scores.Count;
            novel.AverageRating = scores.Count > 0 ? (decimal)scores.Average() : 0m;
            await _context.SaveChangesAsync();
        }

        public async Task AddChapterToNovel(Chapter chapter)
        {
            await _context.Chapters.AddAsync(chapter);
        }

        public async Task<bool> IsNovelBelongsToAuthorAsync(int novelId, int userId)
        {
            return await _context.Novels.AnyAsync(n =>
                n.Id == novelId && n.AuthorProfile.UserId == userId
            );
        }
    }
}
