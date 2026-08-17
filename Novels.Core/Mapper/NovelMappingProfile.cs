using AutoMapper;
using Novels.Core.DTOs.Novel;
using Novels.Domain.Entities;

namespace Novels.Core.Mapper
{
    public class NovelMappingProfile : Profile
    {
        public NovelMappingProfile()
        {
            CreateMap<Novel, NovelDto>()
                // 1. Id
                .ForCtorParam(nameof(NovelDto.Id), opt => opt.MapFrom(src => src.Id))
                // 2. Name
                .ForCtorParam(nameof(NovelDto.Name), opt => opt.MapFrom(src => src.Name))
                // 3. Slug
                .ForCtorParam(nameof(NovelDto.Slug), opt => opt.MapFrom(src => src.Slug))
                // 4. Description
                .ForCtorParam(
                    nameof(NovelDto.Description),
                    opt => opt.MapFrom(src => src.Description)
                )
                // 5. CoverImageUrl
                .ForCtorParam(
                    nameof(NovelDto.CoverImageUrl),
                    opt => opt.MapFrom(src => src.CoverImageUrl)
                )
                // 6. Status
                .ForCtorParam(
                    nameof(NovelDto.Status),
                    opt => opt.MapFrom(src => src.Status.ToString())
                )
                // 7. AuthorProfileId
                .ForCtorParam(
                    nameof(NovelDto.AuthorProfileId),
                    opt => opt.MapFrom(src => src.AuthorProfileId)
                )
                // 8. AuthorName
                .ForCtorParam(
                    nameof(NovelDto.AuthorName),
                    opt => opt.MapFrom(src => src.AuthorProfile.User.UserName)
                )
                // 9. AverageRating
                .ForCtorParam(
                    nameof(NovelDto.AverageRating),
                    opt => opt.MapFrom(src => src.AverageRating)
                )
                // 10. RatingsCount
                .ForCtorParam(
                    nameof(NovelDto.RatingsCount),
                    opt => opt.MapFrom(src => src.RatingsCount)
                )
                // 11. Viewers
                .ForCtorParam(nameof(NovelDto.Viewers), opt => opt.MapFrom(src => src.Viewers))
                // 12. NumOfCollections
                .ForCtorParam(
                    nameof(NovelDto.NumOfCollections),
                    opt => opt.MapFrom(src => src.NumOfCollections)
                )
                // 13. Tags
                .ForCtorParam(
                    nameof(NovelDto.Tags),
                    opt => opt.MapFrom(src => src.Tags.Select(t => t.Name).ToList())
                )
                // 14. Categories
                .ForCtorParam(
                    nameof(NovelDto.Categories),
                    opt => opt.MapFrom(src => src.Categories.Select(c => c.Name).ToList())
                )
                // 15. ChapterCount
                .ForCtorParam(
                    nameof(NovelDto.ChapterCount),
                    opt => opt.MapFrom(src => src.Chapters.Count)
                );
        }
    }
}
