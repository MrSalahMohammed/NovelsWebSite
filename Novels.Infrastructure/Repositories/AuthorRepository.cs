using Microsoft.EntityFrameworkCore;
using Novels.Core.Interfaces.Repositories;
using Novels.Domain.Entities;
using Novels.Infrastructure.Data;

namespace Novels.Infrastructure.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _context;

        public AuthorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuthorProfile?> GetAuthorProfileByUserIdAsync(int userId)
        {
            return await _context.AuthorProfiles.FirstOrDefaultAsync(ap => ap.UserId == userId);
        }

        public void AddAuthorProfile(AuthorProfile authorProfile)
        {
            _context.AuthorProfiles.Add(authorProfile);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
