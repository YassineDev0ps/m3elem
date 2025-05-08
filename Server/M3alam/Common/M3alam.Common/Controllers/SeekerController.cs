using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using M3alem.Domain.Entities;
using M3alem.Services.Seeker.DTOs;
using M3alem.Services.Seeker.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace M3alem.API.Controllers
{
    [ApiController]
    [Route("api/seeker")]
    [Authorize(Policy = "SeekerPolicy")]
    public class SeekerController : ControllerBase
    {
        private readonly ISeekerService _seekerService;

        public SeekerController(ISeekerService seekerService)
        {
            _seekerService = seekerService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ProfileDto>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var seekerId))
            {
                return Unauthorized();
            }

            var profile = await _seekerService.GetProfileAsync(seekerId);
            
            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<ProfileDto>> UpdateProfile([FromBody] UpdateProfileDto updateDto)
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

            var profile = await _seekerService.UpdateProfileAsync(seekerId, updateDto);
            
            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        [HttpGet("requests")]
        public async Task<ActionResult<IEnumerable<RequestSummaryDto>>> GetRequests([FromQuery] RequestStatus? status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var seekerId))
            {
                return Unauthorized();
            }

            var requests = await _seekerService.GetRequestsAsync(seekerId, status);
            return Ok(requests);
        }

        [HttpPut("requests/{id}/cancel")]
        public async Task<ActionResult> CancelRequest(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var seekerId))
            {
                return Unauthorized();
            }

            var result = await _seekerService.CancelRequestAsync(seekerId, id);
            
            if (!result)
            {
                return BadRequest(new { message = "Failed to cancel request" });
            }

            return Ok(new { message = "Request cancelled successfully" });
        }
    }
}