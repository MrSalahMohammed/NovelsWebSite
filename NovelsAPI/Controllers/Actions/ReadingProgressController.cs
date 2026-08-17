using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Novels.Core.DTOs.Reader;
using Novels.Core.Interfaces.Services;

namespace Novels.API.Controllers.Actions
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class ReadingProgressController : BaseApiController
    {
        private readonly IReadingProgressService _readingProgressService;

        public ReadingProgressController(IReadingProgressService readingProgressService)
        {
            _readingProgressService = readingProgressService;
        }

        [HttpGet("progress/{novelId:int}")]
        [Authorize(Roles = Domain.Entities.Roles.Reader + "," + Domain.Entities.Roles.Author)]
        [ProducesResponseType(typeof(ReadingProgressResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReadingProgressResponse>> GetProgress(int novelId)
        {
            var progress = await _readingProgressService.GetProgressAsync(CurrentReaderId, novelId);

            if (progress is null)
                return NotFound(new { message = "No reading progress found for this novel." });

            return Ok(
                new ReadingProgressResponse(
                    progress.NovelId,
                    progress.LastChapterId,
                    progress.LastChapterName,
                    progress.LastReadAt
                )
            );
        }

        [HttpPut("progress/{novelId:int}")]
        [Authorize(Roles = Domain.Entities.Roles.Reader + "," + Domain.Entities.Roles.Author)]
        [ProducesResponseType(typeof(ReadingProgressResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReadingProgressResponse>> UpdateProgress(
            int novelId,
            [FromBody] UpdateProgressRequest request
        )
        {
            var progress = await _readingProgressService.UpdateProgressAsync(
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
                    progress.LastChapterName,
                    progress.LastReadAt
                )
            );
        }
    }
}
