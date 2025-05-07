// M3alam.Web/Controllers/RequestController.cs

using Microsoft.AspNetCore.Mvc;
using MediatR;
using M3alam.Application.Commands;
using M3alam.Domain.Models;
using System.Threading.Tasks;

namespace M3alam.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/Request
        [HttpPost]
        public async Task<ActionResult<Request>> CreateRequest([FromBody] CreateRequestCommand command)
        {
            var request = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetRequestById), new { id = request.Id }, request);
        }

        // GET: api/Request/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequestById(string id)
        {
            // Implement logic to get request by ID
            return Ok();
        }
    }
}
