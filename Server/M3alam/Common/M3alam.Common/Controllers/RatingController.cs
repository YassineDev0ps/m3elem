using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using M3alem.Services.Rating.DTOs;
using M3alem.Services.Rating.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace M3alem.API.Controllers
{
    [ApiController]
    [Route("api/ratings")]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        [Authorize(Policy = "SeekerPolicy")]
        public async Task<ActionResult<RatingDto>> CreateRating([FromBody] CreateRatingDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var seekerId))
            {
                return Unauthorized();
            }

            try
            {
                var rating = await _ratingService.CreateRatingAsync(seekerId, createDto);
                return Ok(rating);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("request/{requestId}")]
        public async Task<ActionResult<RatingDto>> GetRatingForRequest(Guid requestId)
        {
            var rating = await _ratingService.GetRatingForRequestAsync(requestId);
            
            if (rating == null)
            {
                return NotFound();
            }

            return Ok(rating);
        }

        [HttpGet("provider/{providerId}")]
        public async Task<ActionResult<IEnumerable<RatingDto>>> GetProviderRatings(Guid providerId)
        {
            var ratings = await _ratingService.GetProviderRatingsAsync(providerId);
            return Ok(ratings);
        }
    }
}