using AutoMapper;
using Novels.Core.DTOs.Reader;
using Novels.Domain.Entities;

namespace Novels.Core.Mapper
{
    public class ReadingProgressMappingProfile : Profile
    {
        public ReadingProgressMappingProfile()
        {
            CreateMap<ReadingProgress, ReadingProgressDto>()
                // 1. Id
                .ForCtorParam(nameof(ReadingProgressDto.Id), opt => opt.MapFrom(src => src.Id))
                // 2. NovelId
                .ForCtorParam(
                    nameof(ReadingProgressDto.NovelId),
                    opt => opt.MapFrom(src => src.NovelId)
                )
                // 3. NovelName
                .ForCtorParam(
                    nameof(ReadingProgressDto.NovelName),
                    opt => opt.MapFrom(src => src.Novel.Name)
                )
                // 4. NovelSlug
                .ForCtorParam(
                    nameof(ReadingProgressDto.NovelSlug),
                    opt => opt.MapFrom(src => src.Novel.Slug)
                )
                // 5. NovelCoverImageUrl
                .ForCtorParam(
                    nameof(ReadingProgressDto.NovelCoverImageUrl),
                    opt => opt.MapFrom(src => src.Novel.CoverImageUrl)
                )
                // 6. LastChapterId
                .ForCtorParam(
                    nameof(ReadingProgressDto.LastChapterId),
                    opt => opt.MapFrom(src => src.LastChapterId)
                )
                // 7. LastChapterNumber
                .ForCtorParam(
                    nameof(ReadingProgressDto.LastChapterNumber),
                    opt => opt.MapFrom(src => src.LastChapter.ChapterNumber)
                ) // Adjust property name (e.g., Number or Order) as defined in Chapter entity
                  // 8. LastReadAt
                .ForCtorParam(
                    nameof(ReadingProgressDto.LastReadAt),
                    opt => opt.MapFrom(src => src.LastReadAt)
                )
                .ForCtorParam(
                    nameof(ReadingProgressDto.LastChapterName),
                    opt => opt.MapFrom(src => src.LastChapter.Name)
                );
        }
    }
}
