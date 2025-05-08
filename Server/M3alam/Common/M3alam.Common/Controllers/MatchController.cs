using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using M3alem.Services.Matching.DTOs;
using M3alem.Services.Matching.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace M3alem.API.Controllers
{
    [ApiController]
    [Route("api/match")]
    [Authorize(Policy = "SeekerPolicy")]
    public class MatchController : ControllerBase
    {
        private readonly IMatchingService _matchingService;

        public MatchController(IMatchingService matchingService)
        {
            _matchingService = matchingService;
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<ProviderMatchDto>>> FindMatches([FromBody] MatchRequestDto matchRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var providers = await _matchingService.FindMatchingProvidersAsync(matchRequest);
            return Ok(providers);
        }
    }
}