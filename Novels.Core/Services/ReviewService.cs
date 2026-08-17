using AutoMapper;
using Novels.Core.DTOs.Reader;
using Novels.Core.Interfaces.Repositories;
using Novels.Core.Interfaces.Services;
using Novels.Domain.Entities;

namespace Novels.Core.Services
{
    internal class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly INovelRepository _novelRepository;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewRepository reviewRepository,
            IMapper mapper,
            INovelRepository novelRepository
        )
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _novelRepository = novelRepository;
        }

        public async Task<ReviewDto?> AddOrUpdateReviewAsync(
            int readerId,
            int novelId,
            ReviewRequest request
        )
        {
            if (request.Score is < 1 or > 5)
                return null;

            bool novelExists = await _novelRepository.NovelExistsAsync(novelId);
            if (!novelExists)
                return null;

            var review = await _reviewRepository.GetReviewByUserAndNovelAsync(readerId, novelId);

            if (review is null)
            {
                review = new Review
                {
                    ReaderId = readerId,
                    NovelId = novelId,
                    Score = request.Score,
                    Comment = request.Comment,
                    CreatedAt = DateTime.UtcNow,
                };
                _reviewRepository.AddReview(review);
            }
            else
            {
                review.Score = request.Score;
                review.Comment = request.Comment;
            }

            await _reviewRepository.SaveChangesAsync();
            await _novelRepository.RecalculateNovelRatingAsync(novelId);
            await _novelRepository.SaveChangesAsync();

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<bool> DeleteReviewAsync(int readerId, int novelId)
        {
            bool novelExists = await _novelRepository.NovelExistsAsync(novelId);
            if (!novelExists)
                return false;

            var review = await _reviewRepository.GetReviewByUserAndNovelAsync(readerId, novelId);
            if (review is null)
                return false;

            _reviewRepository.RemoveReview(review);
            await _reviewRepository.SaveChangesAsync();
            await _novelRepository.RecalculateNovelRatingAsync(novelId);
            await _novelRepository.SaveChangesAsync();
            return true;
        }
    }
}
