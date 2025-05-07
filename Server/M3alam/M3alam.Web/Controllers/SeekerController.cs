// M3alam.Web/Controllers/SeekerController.cs

using Microsoft.AspNetCore.Mvc;
using MediatR;
using M3alam.Application.Commands;
using M3alam.Domain.Models;
using System.Threading.Tasks;

namespace M3alam.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeekerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SeekerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/Seeker
        [HttpPost]
        public async Task<ActionResult<Seeker>> CreateSeeker([FromBody] CreateSeekerCommand command)
        {
            var seeker = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetSeekerById), new { id = seeker.Id }, seeker);
        }

        // GET: api/Seeker/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Seeker>> GetSeekerById(string id)
        {
            // Implement logic to get seeker by ID
            return Ok();
        }
    }
}
