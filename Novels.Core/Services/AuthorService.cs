using Novels.Core.DTOs.Reader;
using Novels.Core.Interfaces.Repositories;
using Novels.Core.Interfaces.Services;
using Novels.Domain.Entities;

namespace Novels.Core.Services
{
    internal class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IReaderRepository _readerRepository;

        public AuthorService(IAuthorRepository authorRepository, IReaderRepository readerRepository)
        {
            _authorRepository = authorRepository;
            _readerRepository = readerRepository;
        }

        public async Task<bool> PromoteToAuthorAsync(int readerId, PromoteToAuthorRequest request)
        {
            var user = await _readerRepository.GetReaderByID(readerId);
            if (user is null)
                return false;

            // 2. Manage identity roles via repository
            bool isAuthor = await _readerRepository.IsUserInRoleAsync(user, Roles.Author);
            if (!isAuthor)
            {
                bool roleAdded = await _readerRepository.AddUserToRoleAsync(user, Roles.Author);
                if (!roleAdded)
                    return false;
            }

            // 3. Create or update the Author Profile
            var existingProfile = await _authorRepository.GetAuthorProfileByUserIdAsync(readerId);

            if (existingProfile is null)
            {
                var newProfile = new AuthorProfile { UserId = readerId, Bio = request.Bio };

                // Synchronous memory operation
                _authorRepository.AddAuthorProfile(newProfile);
            }
            else if (request.Bio is not null)
            {
                existingProfile.Bio = request.Bio;
            }

            // 4. Persist database changes
            await _authorRepository.SaveChangesAsync();
            return true;
        }
    }
}
