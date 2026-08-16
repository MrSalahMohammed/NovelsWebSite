using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Novels.Core.Interfaces.Services;
using Novels.Domain.Entities;
using Novels.Infrastructure.Data;

namespace Novels.Core.Services
{
    public class ReaderService : IReaderService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReaderService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ---------- Favorites ----------

        public async Task<bool> AddFavoriteAsync(int readerId, int novelId)
        {
            var reader = await _context
                .Users.Include(u => u.FavoriteNovels)
                .FirstOrDefaultAsync(u => u.Id == readerId);
            if (reader is null)
                return false;

            var novel = await _context.Novels.FindAsync(novelId);
            if (novel is null)
                return false;

            if (reader.FavoriteNovels.Any(n => n.Id == novelId))
                return true; // already a favorite — idempotent, not an error

            reader.FavoriteNovels.Add(novel);
            novel.NumOfCollections++; // keep the cached counter in sync
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFavoriteAsync(int readerId, int novelId)
        {
            var reader = await _context
                .Users.Include(u => u.FavoriteNovels)
                .FirstOrDefaultAsync(u => u.Id == readerId);
            if (reader is null)
                return false;

            var novel = reader.FavoriteNovels.FirstOrDefault(n => n.Id == novelId);
            if (novel is null)
                return true; // wasn't a favorite — idempotent, not an error

            reader.FavoriteNovels.Remove(novel);
            if (novel.NumOfCollections > 0)
                novel.NumOfCollections--;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Novel>> GetFavoritesAsync(int readerId)
        {
            return await _context
                .Users.Where(u => u.Id == readerId)
                .SelectMany(u => u.FavoriteNovels)
                .AsNoTracking()
                .ToListAsync();
        }

        // ---------- Reading progress ----------

        public async Task<ReadingProgress?> GetProgressAsync(int readerId, int novelId)
        {
            return await _context
                .ReadingProgresses.AsNoTracking()
                .FirstOrDefaultAsync(rp => rp.ReaderId == readerId && rp.NovelId == novelId);
        }

        public async Task<ReadingProgress?> UpdateProgressAsync(
            int readerId,
            int novelId,
            int chapterId
        )
        {
            var chapterBelongsToNovel = await _context.Chapters.AnyAsync(c =>
                c.Id == chapterId && c.NovelId == novelId
            );
            if (!chapterBelongsToNovel)
                return null; // chapter doesn't exist or belongs to a different novel

            var progress = await _context.ReadingProgresses.FirstOrDefaultAsync(rp =>
                rp.ReaderId == readerId && rp.NovelId == novelId
            );

            if (progress is null)
            {
                progress = new ReadingProgress
                {
                    ReaderId = readerId,
                    NovelId = novelId,
                    LastChapterId = chapterId,
                    LastReadAt = DateTime.UtcNow,
                };
                _context.ReadingProgresses.Add(progress);
            }
            else
            {
                progress.LastChapterId = chapterId;
                progress.LastReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return progress;
        }

        // ---------- Reviews ----------

        public async Task<Review?> AddOrUpdateReviewAsync(
            int readerId,
            int novelId,
            int score,
            string? comment
        )
        {
            if (score is < 1 or > 5)
                return null; // caller (controller) turns this into a 400

            var novelExists = await _context.Novels.AnyAsync(n => n.Id == novelId);
            if (!novelExists)
                return null;

            var review = await _context.Reviews.FirstOrDefaultAsync(r =>
                r.ReaderId == readerId && r.NovelId == novelId
            );

            if (review is null)
            {
                review = new Review
                {
                    ReaderId = readerId,
                    NovelId = novelId,
                    Score = score,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow,
                };
                _context.Reviews.Add(review);
            }
            else
            {
                review.Score = score;
                review.Comment = comment;
            }

            await _context.SaveChangesAsync();
            await RecalculateNovelRatingAsync(novelId);
            return review;
        }

        public async Task<bool> DeleteReviewAsync(int readerId, int novelId)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r =>
                r.ReaderId == readerId && r.NovelId == novelId
            );
            if (review is null)
                return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            await RecalculateNovelRatingAsync(novelId);
            return true;
        }

        // Keeps Novel.AverageRating / RatingsCount in sync from the Reviews table —
        // this is the "cached, recomputed value" note from the earlier domain model review.
        private async Task RecalculateNovelRatingAsync(int novelId)
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

        // ---------- Account management ----------

        public async Task<bool> UpdateReaderDataAsync(
            int readerId,
            string fName,
            string lName,
            string? recoveryEmail,
            string? phoneNumber
        )
        {
            var user = await _userManager.FindByIdAsync(readerId.ToString());
            if (user is null)
                return false;

            user.FName = fName;
            user.LName = lName;
            user.RecoveryEmail = recoveryEmail;
            user.PhoneNumber = phoneNumber;

            var result = await _userManager.UpdateAsync(user); // goes through Identity, not raw SaveChanges
            return result.Succeeded;
        }

        public async Task<bool> PromoteToAuthorAsync(int readerId, string? bio)
        {
            var user = await _userManager.FindByIdAsync(readerId.ToString());
            if (user is null)
                return false;

            if (!await _userManager.IsInRoleAsync(user, Roles.Author))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, Roles.Author);
                if (!roleResult.Succeeded)
                    return false;
            }

            var existingProfile = await _context.AuthorProfiles.FirstOrDefaultAsync(ap =>
                ap.UserId == readerId
            );

            if (existingProfile is null)
            {
                _context.AuthorProfiles.Add(new AuthorProfile { UserId = readerId, Bio = bio });
            }
            else if (bio is not null)
            {
                existingProfile.Bio = bio; // update bio if a new one was provided, idempotent otherwise
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReaderAsync(int readerId)
        {
            var user = await _userManager.FindByIdAsync(readerId.ToString());
            if (user is null)
                return false;

            // Soft "delete" via lockout — no schema change needed, and it's reversible.
            // The user row, their reviews, favorites, and reading history all stay intact.
            await _userManager.SetLockoutEnabledAsync(user, true);
            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            return result.Succeeded;
        }

        public async Task<bool> ReactivateReaderAsync(int readerId)
        {
            var user = await _userManager.FindByIdAsync(readerId.ToString());
            if (user is null)
                return false;

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            return result.Succeeded;
        }

        public async Task<List<ReadingProgress>> GetReadingHistoryAsync(int readerId)
        {
            return await _context
                .ReadingProgresses.Where(rp => rp.ReaderId == readerId)
                .Include(rp => rp.Novel)
                .Include(rp => rp.LastChapter)
                .OrderByDescending(rp => rp.LastReadAt)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
