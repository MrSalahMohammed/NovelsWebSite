using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Novels.Core.DTOs.Novel;
using Novels.Core.DTOs.Reader;
using Novels.Core.Interfaces.Services;

namespace Novels.API.Controllers.Roles
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class AuthorController : BaseApiController
    {
        private readonly IAuthorService _authorService;
        private readonly INovelService _novelService;
        private readonly IValidator<AddChapterRequest> _validator;

        public AuthorController(
            IAuthorService authorService,
            INovelService novelService,
            IValidator<AddChapterRequest> validator
        )
        {
            _authorService = authorService;
            _novelService = novelService;
            _validator = validator;
        }

        [HttpPost("me/become-author")]
        [Authorize(Roles = Domain.Entities.Roles.Reader)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> BecomeAuthor([FromBody] PromoteToAuthorRequest request)
        {
            if (CurrentReaderId == 0)
                return Unauthorized("User ID claim is missing or invalid.");

            var success = await _authorService.PromoteToAuthorAsync(CurrentReaderId, request);

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

        [HttpPut("Novel/Add-Chapter")]
        [Authorize(Roles = Domain.Entities.Roles.Author)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddChapter([FromBody] AddChapterRequest request)
        {
            if (CurrentReaderId == 0)
                return Unauthorized("User ID claim is missing or invalid.");

            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));
            }

            var success = await _novelService.AddChapterToNovel(request, CurrentReaderId);
            if (!success)
            {
                return BadRequest(new { message = "Unable to Add Chapter!" });
            }

            return NoContent();
        }
    }
}
