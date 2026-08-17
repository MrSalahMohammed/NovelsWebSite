using Microsoft.EntityFrameworkCore;
using Novels.Core.Interfaces.Repositories;
using Novels.Domain.Entities;
using Novels.Infrastructure.Data;

namespace Novels.Infrastructure.Repositories
{
    internal class ReadingProgressRepository : IReadingProgressRepository
    {
        private readonly AppDbContext _context;

        public ReadingProgressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReadingProgress?> GetProgressAsync(int readerId, int novelId)
        {
            return await _context
                .ReadingProgresses.Include(r => r.LastChapter)
                .AsNoTracking()
                .FirstOrDefaultAsync(rp => rp.ReaderId == readerId && rp.NovelId == novelId);
        }

        public void AddProgress(ReadingProgress progress)
        {
            _context.ReadingProgresses.Add(progress);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
