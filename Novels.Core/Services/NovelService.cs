using Novels.Core.DTOs.Novel;
using Novels.Core.Interfaces.Repositories;
using Novels.Core.Interfaces.Services;
using Novels.Domain.Entities;

namespace Novels.Core.Services
{
    public class NovelService : INovelService
    {
        private readonly INovelRepository _novelRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly IReaderRepository _readerRepository;

        public NovelService(
            INovelRepository novelRepository,
            IAuthorRepository authorRepository,
            IReaderRepository readerRepository
        )
        {
            _novelRepository = novelRepository;
            _authorRepository = authorRepository;
            _readerRepository = readerRepository;
        }

        public async Task<bool> AddChapterToNovel(AddChapterRequest request, int userId)
        {
            var user = _readerRepository.GetReaderByID(userId);
            if (user is null)
                return false;

            var Novel = _novelRepository.FindNovelByID(request.NovelId);
            if (Novel is null)
            {
                return false;
            }

            if (
                await _novelRepository.IsNovelBelongsToAuthorAsync(request.NovelId, userId) == false
            )
            {
                return false;
            }

            var chapter = new Chapter
            {
                NovelId = request.NovelId,
                Name = request.Name,
                Content = request.Content,
                IsPublished = true,
                CreatedAt = DateTime.Now,
            };

            await _novelRepository.AddChapterToNovel(chapter);
            await _novelRepository.SaveChangesAsync();
            return true;
        }
    }
}
