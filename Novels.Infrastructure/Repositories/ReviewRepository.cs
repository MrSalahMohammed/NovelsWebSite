using Microsoft.EntityFrameworkCore;
using Novels.Core.Interfaces.Repositories;
using Novels.Domain.Entities;
using Novels.Infrastructure.Data;

namespace Novels.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetReviewByUserAndNovelAsync(int readerId, int novelId)
        {
            return await _context.Reviews.FirstOrDefaultAsync(r =>
                r.ReaderId == readerId && r.NovelId == novelId
            );
        }

        public void AddReview(Review review)
        {
            _context.Reviews.Add(review);
        }

        public void RemoveReview(Review review)
        {
            _context.Reviews.Remove(review);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
