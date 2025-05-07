
using M3alam.Common.DataTransferObjects;
using M3alam.Identity.Application.Commands;
using M3alam.Identity.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace M3alam.Identity.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public class IdentityController : Controller
    {
        private readonly IMediator _mediator;
        public IdentityController(IMediator mediator) => _mediator = mediator;

        // POST: /api/users/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand cmd)
        {
            await _mediator.Send(cmd);
            return CreatedAtAction(nameof(GetById), new { id = Guid.NewGuid() }, null);
        }

       

        // GET: /api/users
        [HttpGet]
        public async Task<ActionResult<UserDto[]>> GetAll()
        {
            var list = await _mediator.Send(new GetAllUsersQuery());
            return Ok(list);
        }

        // GET: /api/users/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery { UserId = id });
            if (user == null) return NotFound();
            return Ok(user);
        }

        // PUT: /api/users/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand cmd)
        {
            cmd.UserId = id;
            var ok = await _mediator.Send(cmd);
            if (!ok) return BadRequest();
            return NoContent();
        }

        // POST: /api/users/{id}/change-password
        [HttpPost("{id:guid}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordCommand cmd)
        {
            cmd.UserId = id;
            var ok = await _mediator.Send(cmd);
            if (!ok) return BadRequest();
            return NoContent();
        }

        //// POST: /api/users/forgot-password
        //[AllowAnonymous]
        //[HttpPost("forgot-password")]
        //public async Task<IActionResult> ForgotPwd([FromBody] ForgotPasswordCommand cmd)
        //{
        //    await _mediator.Send(cmd);
        //    return Accepted();
        //}

        // POST: /api/users/reset-password
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPwd([FromBody] ResetPasswordCommand cmd)
        {
            var ok = await _mediator.Send(cmd);
            if (!ok) return BadRequest();
            return NoContent();
        }
    }
}
