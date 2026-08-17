using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Novels.Core.Interfaces.Repositories;
using Novels.Domain.Entities;
using Novels.Infrastructure.Data;

namespace Novels.Infrastructure.Repositories
{
    public class ReaderRepository : IReaderRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReaderRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetReaderByID(int readerId)
        {
            return await _context
                .Users.Include(u => u.FavoriteNovels)
                .FirstOrDefaultAsync(u => u.Id == readerId);
        }

        public async Task<bool> UpdateReaderDataAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteReader(ApplicationUser user)
        {
            await _userManager.SetLockoutEnabledAsync(user, true);
            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            return result.Succeeded;
        }

        public async Task<bool> IsUserInRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> AddUserToRoleAsync(ApplicationUser user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
