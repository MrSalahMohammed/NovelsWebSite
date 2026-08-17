using AutoMapper;
using Novels.Core.DTOs.Reader;
using Novels.Core.Interfaces.Repositories;
using Novels.Core.Interfaces.Services;
using Novels.Domain.Entities;

namespace Novels.Core.Services
{
    internal class ReadingProgressService : IReadingProgressService
    {
        private readonly INovelRepository _novelRepository;
        private readonly IReadingProgressRepository _readingProgressRepository;
        private readonly IMapper _mapper;

        public ReadingProgressService(
            IReadingProgressRepository readingProgressRepository,
            INovelRepository novelRepository,
            IMapper mapper
        )
        {
            _novelRepository = novelRepository;
            _readingProgressRepository = readingProgressRepository;
            _mapper = mapper;
        }

        public async Task<ReadingProgressDto?> GetProgressAsync(int readerId, int novelId)
        {
            var ReadingProgress = await _readingProgressRepository.GetProgressAsync(
                readerId,
                novelId
            );
            return _mapper.Map<ReadingProgressDto>(ReadingProgress);
        }

        public async Task<ReadingProgressDto?> UpdateProgressAsync(
            int readerId,
            int novelId,
            int chapterId
        )
        {
            bool isValidChapter = await _novelRepository.ChapterBelongsToNovelAsync(
                chapterId,
                novelId
            );
            if (!isValidChapter)
                return null;

            var progress = await _readingProgressRepository.GetProgressAsync(readerId, novelId);

            if (progress is null)
            {
                progress = new ReadingProgress
                {
                    ReaderId = readerId,
                    NovelId = novelId,
                    LastChapterId = chapterId,
                    LastReadAt = DateTime.UtcNow,
                };
                _readingProgressRepository.AddProgress(progress);
            }
            else
            {
                progress.LastChapterId = chapterId;
                progress.LastReadAt = DateTime.UtcNow;
            }

            await _readingProgressRepository.SaveChangesAsync();

            return _mapper.Map<ReadingProgressDto>(progress);
        }
    }
}
