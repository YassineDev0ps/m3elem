// M3alam.Web/Controllers/ProviderController.cs

using Microsoft.AspNetCore.Mvc;
using MediatR;
using M3alam.Application.Commands;
using M3alam.Domain.Models;
using System.Threading.Tasks;

namespace M3alam.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProviderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/Provider
        [HttpPost]
        public async Task<ActionResult<Provider>> CreateProvider([FromBody] CreateProviderCommand command)
        {
            var provider = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProviderById), new { id = provider.Id }, provider);
        }

        // GET: api/Provider/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Provider>> GetProviderById(string id)
        {
            // Implement logic to get provider by ID
            return Ok();
        }
    }
}
