using Microsoft.AspNetCore.Mvc;
using MediatR;
using M3alam.Application.Commands;
using M3alam.Domain.Models;
using System.Threading.Tasks;

namespace M3alam.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserCommand command)
        {
            var user = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            // Logic to get user by ID if needed
            var user = await _mediator.Send(new GetUserByIdCommand { Id = id });
            return user != null ? Ok(user) : NotFound();
        }
    }
}

