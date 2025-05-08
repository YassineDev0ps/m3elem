using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using M3alem.Services.Chat.DTOs;
using M3alem.Services.Chat.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace M3alem.API.Controllers
{
    [ApiController]
    [Route("api/messages")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("{requestId}")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetMessageHistory(Guid requestId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized();
            }

            try
            {
                // Check if user can access this chat
                var canAccess = await _chatService.UserCanAccessChatAsync(userGuid, requestId);
                if (!canAccess)
                {
                    return Forbid();
                }

                var messages = await _chatService.GetMessageHistoryAsync(requestId, userGuid);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{requestId}")]
        public async Task<ActionResult<ChatMessageDto>> SendMessage(Guid requestId, [FromBody] SendMessageDto messageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized();
            }

            // Ensure the requestId in route matches the one in DTO
            if (requestId != messageDto.RequestId)
            {
                return BadRequest(new { message = "Request ID mismatch" });
            }

            try
            {
                var message = await _chatService.SendMessageAsync(userGuid, messageDto);
                return Ok(message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}