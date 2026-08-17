using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Novels.Core.DTOs.Reader;
using Novels.Core.Interfaces.Services;

namespace Novels.API.Controllers.Actions
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : BaseApiController
    {
        private readonly IReviewService _reviewService;
        private readonly IValidator<ReviewRequest> _reviewValidator;

        public ReviewController(
            IReviewService reviewService,
            IValidator<ReviewRequest> reviewValidator
        )
        {
            _reviewService = reviewService;
            _reviewValidator = reviewValidator;
        }

        [HttpPut("reviews/{novelId:int}")]
        [Authorize(Roles = Domain.Entities.Roles.Reader + "," + Domain.Entities.Roles.Author)]
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

            var review = await _reviewService.AddOrUpdateReviewAsync(
                CurrentReaderId,
                novelId,
                request
            );

            if (review is null)
                return NotFound(new { message = $"Novel with ID {novelId} was not found." });

            return Ok(
                new ReviewResponse(review.NovelId, review.Score, review.Comment, review.CreatedAt)
            );
        }

        [HttpDelete("reviews/{novelId:int}")]
        [Authorize(Roles = Domain.Entities.Roles.Reader + "," + Domain.Entities.Roles.Author)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview(int novelId)
        {
            var success = await _reviewService.DeleteReviewAsync(CurrentReaderId, novelId);

            if (!success)
                return NotFound(new { message = "Review not found or already deleted." });

            return NoContent();
        }
    }
}
