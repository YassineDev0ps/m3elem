using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using M3alem.Services.Request.DTOs;
using M3alem.Services.Request.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace M3alem.API.Controllers
{
    [ApiController]
    [Route("api/requests")]
    [Authorize]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost]
        [Authorize(Policy = "SeekerPolicy")]
        public async Task<ActionResult<Guid>> CreateRequest([FromBody] CreateRequestDto createDto)
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
                var requestId = await _requestService.CreateRequestAsync(seekerId, createDto);
                return Ok(new { requestId });
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

        [HttpGet("{id}")]
        public async Task<ActionResult<RequestDetailDto>> GetRequestDetail(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var request = await _requestService.GetRequestDetailAsync(id);
            
            if (request == null)
            {
                return NotFound();
            }

            // Verify user has permission to view this request
            if (request.SeekerId.ToString() != userId && request.SelectedProviderId.ToString() != userId)
            {
                return Forbid();
            }

            return Ok(request);
        }

        [HttpPost("{id}/complete")]
        [Authorize(Policy = "ProviderPolicy")]
        public async Task<ActionResult> MarkRequestCompleted(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // First get the request to check if this provider is authorized
            var request = await _requestService.GetRequestDetailAsync(id);
            
            if (request == null)
            {
                return NotFound();
            }

            // Verify user is the assigned provider
            if (request.SelectedProviderId.ToString() != userId)
            {
                return Forbid();
            }

            var result = await _requestService.MarkRequestCompletedAsync(id);
            
            if (!result)
            {
                return BadRequest(new { message = "Failed to mark request as completed" });
            }

            return Ok(new { message = "Request marked as completed successfully" });
        }
    }
}