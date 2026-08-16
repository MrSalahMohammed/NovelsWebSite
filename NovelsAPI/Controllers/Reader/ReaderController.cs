using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Novels.Core.DTOs.Reader;
using Novels.Core.Interfaces.Services;
using Novels.Domain.Entities;

namespace Novels.API.Controllers.Reader
{
    [Authorize]
    [ApiController]
    [Route("api/Reader")]
    public class ReaderController : ControllerBase
    {
        private readonly IReaderService _readerService;
        private readonly IValidator<ReviewRequest> _reviewValidator;
        private readonly IValidator<UpdateReaderRequest> _updateReaderValidator;

        public ReaderController(
            IReaderService readerService,
            IValidator<ReviewRequest> reviewValidator,
            IValidator<UpdateReaderRequest> updateReaderValidator
        )
        {
            _readerService = readerService;
            _reviewValidator = reviewValidator;
            _updateReaderValidator = updateReaderValidator;
        }

        private int CurrentReaderId
        {
            get
            {
                var claimValue =
                    User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

                if (int.TryParse(claimValue, out var readerId))
                {
                    return readerId;
                }

                throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
            }
        }

        [HttpPost("favorites/{novelId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddFavorite(int novelId)
        {
            var success = await _readerService.AddFavoriteAsync(CurrentReaderId, novelId);

            if (!success)
                return NotFound(new { message = $"Novel with ID {novelId} was not found." });

            return NoContent();
        }

        [HttpDelete("favorites/{novelId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveFavorite(int novelId)
        {
            var success = await _readerService.RemoveFavoriteAsync(CurrentReaderId, novelId);

            if (!success)
                return NotFound(new { message = "Favorite entry not found." });

            return NoContent();
        }

        [HttpGet("favorites")]
        [ProducesResponseType(typeof(IEnumerable<NovelSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NovelSummaryResponse>>> GetFavorites()
        {
            var novels = await _readerService.GetFavoritesAsync(CurrentReaderId);

            var response = novels.Select(n => new NovelSummaryResponse(
                n.Id,
                n.Name,
                n.Slug,
                n.AverageRating,
                n.CoverImageUrl
            ));

            return Ok(response);
        }

        // ---------- Reading Progress ----------

        [HttpGet("progress/{novelId:int}")]
        [ProducesResponseType(typeof(ReadingProgressResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReadingProgressResponse>> GetProgress(int novelId)
        {
            var progress = await _readerService.GetProgressAsync(CurrentReaderId, novelId);

            if (progress is null)
                return NotFound(new { message = "No reading progress found for this novel." });

            return Ok(
                new ReadingProgressResponse(
                    progress.NovelId,
                    progress.LastChapterId,
                    progress.LastReadAt
                )
            );
        }

        [HttpPut("progress/{novelId:int}")]
        [ProducesResponseType(typeof(ReadingProgressResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReadingProgressResponse>> UpdateProgress(
            int novelId,
            [FromBody] UpdateProgressRequest request
        )
        {
            var progress = await _readerService.UpdateProgressAsync(
                CurrentReaderId,
                novelId,
                request.ChapterId
            );

            if (progress is null)
                return BadRequest(
                    new
                    {
                        message = "Invalid chapter or the chapter does not belong to this novel.",
                    }
                );

            return Ok(
                new ReadingProgressResponse(
                    progress.NovelId,
                    progress.LastChapterId,
                    progress.LastReadAt
                )
            );
        }

        // ---------- Reviews ----------

        [HttpPut("reviews/{novelId:int}")]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewResponse>> AddOrUpdateReview(
            int novelId,
            [FromBody] ReviewRequest request
        )
        {
            var validation = await _reviewValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));
            }

            var review = await _readerService.AddOrUpdateReviewAsync(
                CurrentReaderId,
                novelId,
                request.Score,
                request.Comment
            );

            if (review is null)
                return NotFound(new { message = $"Novel with ID {novelId} was not found." });

            return Ok(
                new ReviewResponse(review.NovelId, review.Score, review.Comment, review.CreatedAt)
            );
        }

        [HttpDelete("reviews/{novelId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview(int novelId)
        {
            var success = await _readerService.DeleteReviewAsync(CurrentReaderId, novelId);

            if (!success)
                return NotFound(new { message = "Review not found or already deleted." });

            return NoContent();
        }

        // ---------- Account Management ----------

        [HttpPut("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateReaderData([FromBody] UpdateReaderRequest request)
        {
            var validation = await _updateReaderValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));
            }

            var success = await _readerService.UpdateReaderDataAsync(
                CurrentReaderId,
                request.FName,
                request.LName,
                request.RecoveryEmail,
                request.PhoneNumber
            );

            if (!success)
                return NotFound(new { message = "Reader profile not found or update failed." });

            return NoContent();
        }

        [HttpPost("me/become-author")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> BecomeAuthor([FromBody] PromoteToAuthorRequest request)
        {
            if (CurrentReaderId == 0)
                return Unauthorized("User ID claim is missing or invalid.");

            var success = await _readerService.PromoteToAuthorAsync(CurrentReaderId, request.Bio);

            // 2. Return BadRequest or Conflict if the promotion fails (e.g., user is already an author)
            if (!success)
            {
                return BadRequest(
                    new
                    {
                        message = "Unable to promote account. User may already be an author or account was not found.",
                    }
                );
            }

            return NoContent();
        }

        [HttpDelete("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteMyAccount()
        {
            if (CurrentReaderId == 0)
                return Unauthorized("User ID claim is missing or invalid.");

            var success = await _readerService.DeleteReaderAsync(CurrentReaderId);

            if (!success)
            {
                return BadRequest(
                    new
                    {
                        message = "Could not delete account. The user account does not exist or is already deactivated.",
                    }
                );
            }

            return NoContent();
        }

        [HttpPost("{readerId:int}/reactivate")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ReactivateReader(int readerId)
        {
            if (readerId <= 0)
                return BadRequest(new { message = "Invalid reader ID." });

            var success = await _readerService.ReactivateReaderAsync(readerId);

            if (!success)
            {
                return NotFound(
                    new
                    {
                        message = $"Reader with ID {readerId} was not found or is already active.",
                    }
                );
            }

            return NoContent();
        }

        [HttpGet("history")]
        [ProducesResponseType(
            typeof(IEnumerable<ReadingHistoryItemResponse>),
            StatusCodes.Status200OK
        )]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ReadingHistoryItemResponse>>> GetHistory()
        {
            var history = await _readerService.GetReadingHistoryAsync(CurrentReaderId);

            var response = history.Select(rp => new ReadingHistoryItemResponse(
                rp.NovelId,
                rp.Novel?.Name ?? string.Empty,
                rp.LastChapterId,
                rp.LastChapter?.Name ?? string.Empty,
                rp.LastReadAt
            ));

            return Ok(response);
        }
    }
}
