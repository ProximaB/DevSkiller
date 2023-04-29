using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StatsCounter.Models;
using StatsCounter.Services;

namespace StatsCounter.Controllers
{
    [Route("repositories")]
    [ApiController]
    public class RepositoriesController : ControllerBase
    {
        private readonly IStatsService _statsService;

        public RepositoriesController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet("{owner}")]
        [ProducesResponseType(typeof(RepositoryStats), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RepositoryStats>> Get(
            [FromRoute] string owner)
        {
            if (string.IsNullOrWhiteSpace(owner) || !owner.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
            {
                return BadRequest();
            }

            var result = await _statsService.GetRepositoryStatsByOwnerAsync(owner, CancellationToken.None).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}