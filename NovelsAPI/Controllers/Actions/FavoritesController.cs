using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Novels.Core.DTOs.Reader;
using Novels.Core.Interfaces.Services;

namespace Novels.API.Controllers.Actions
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : BaseApiController
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("favorites/{novelId:int}")]
        [Authorize(Roles = Domain.Entities.Roles.Reader + "," + Domain.Entities.Roles.Author)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddFavorite(int novelId)
        {
            var success = await _favoriteService.AddFavoriteAsync(CurrentReaderId, novelId);

            if (!success)
                return NotFound(new { message = $"Novel with ID {novelId} was not found." });

            return NoContent();
        }

        [HttpDelete("favorites/{novelId:int}")]
        [Authorize(Roles = Domain.Entities.Roles.Reader + "," + Domain.Entities.Roles.Author)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveFavorite(int novelId)
        {
            var success = await _favoriteService.RemoveFavoriteAsync(CurrentReaderId, novelId);

            if (!success)
                return NotFound(new { message = "Favorite entry not found." });

            return NoContent();
        }

        [HttpGet("favorites")]
        [Authorize(Roles = Domain.Entities.Roles.Reader + "," + Domain.Entities.Roles.Author)]
        [ProducesResponseType(typeof(IEnumerable<NovelSummaryResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NovelSummaryResponse>>> GetFavorites()
        {
            var novels = await _favoriteService.GetFavoritesAsync(CurrentReaderId);

            var response = novels.Select(n => new NovelSummaryResponse(
                n.Id,
                n.Name,
                n.Slug,
                n.AverageRating,
                n.CoverImageUrl
            ));

            return Ok(response);
        }
    }
}
