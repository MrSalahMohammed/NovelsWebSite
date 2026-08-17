using AutoMapper;
using Novels.Core.DTOs.Novel;
using Novels.Core.Interfaces.Repositories;
using Novels.Core.Interfaces.Services;

namespace Novels.Core.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IReaderRepository _readerRepository;
        private readonly IFavouritRepository _FavoriteRepository;
        private readonly INovelRepository _novelRepository;
        private readonly IMapper _mapper;

        public FavoriteService(
            IReaderRepository readerRepository,
            IFavouritRepository FavoriteRepository,
            INovelRepository novelRepository,
            IMapper mapper
        )
        {
            _readerRepository = readerRepository;
            _FavoriteRepository = FavoriteRepository;
            _novelRepository = novelRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddFavoriteAsync(int readerId, int novelId)
        {
            bool isAlreadyFavorite = await _FavoriteRepository.IsNovelInUserFavoritesAsync(
                readerId,
                novelId
            );
            if (isAlreadyFavorite)
                return false;

            var reader = await _readerRepository.GetReaderByID(readerId);
            if (reader is null)
                return false;

            var novel = await _novelRepository.FindNovelByID(novelId);
            if (novel is null)
                return false;

            _FavoriteRepository.AddFavoriteAsync(reader, novel);

            await _readerRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveFavoriteAsync(int readerId, int novelId)
        {
            var reader = await _readerRepository.GetReaderByID(readerId);
            if (reader is null)
                return false;

            var novel = await _novelRepository.FindNovelByID(novelId);
            if (novel is null)
                return false;

            _FavoriteRepository.RemoveFavoriteAsync(reader, novel);
            await _readerRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<NovelDto>> GetFavoritesAsync(int readerId)
        {
            var novels = await _FavoriteRepository.GetFavoritesAsync(readerId);
            return _mapper.Map<List<NovelDto>>(novels);
        }
    }
}
