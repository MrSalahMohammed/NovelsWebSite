using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Novels.Core.DTOs.Reader;
using Novels.Core.Interfaces.Services;

namespace Novels.API.Controllers.Roles
{
    [Authorize]
    [ApiController]
    [Route("api/Reader")]
    public class ReaderController : BaseApiController
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

        [HttpPut("me")]
        [Authorize(Roles = Domain.Entities.Roles.Reader + "," + Domain.Entities.Roles.Author)]
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

            var success = await _readerService.UpdateReaderDataAsync(CurrentReaderId, request);

            if (!success)
                return NotFound(new { message = "Reader profile not found or update failed." });

            return NoContent();
        }

        [HttpDelete("me")]
        [Authorize(Roles = Domain.Entities.Roles.Reader + "," + Domain.Entities.Roles.Author)]
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

        [HttpGet("history")]
        [Authorize(Roles = Domain.Entities.Roles.Reader + "," + Domain.Entities.Roles.Author)]
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
                rp.NovelName ?? string.Empty,
                rp.LastChapterId,
                rp.LastChapterName,
                rp.LastReadAt
            ));

            return Ok(response);
        }
    }
}
