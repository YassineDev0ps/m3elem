// M3alam.Web/Controllers/UserController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using M3alam.Common.DataTransferObjects;
using System.Threading.Tasks;

namespace M3alam.Web.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ApiController
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            var command = new RegisterUserCommand(registerUserDto);
            var result = await _mediator.Send(command);

            if (result)
                return Ok("User registered successfully.");

            return BadRequest("User registration failed.");
        }
    }
}
