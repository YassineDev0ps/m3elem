using MediatR;
using M3alam.Infrastructure.Context;
using M3alam.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace M3alam.Application.Handlers
{
    public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, Request>
    {
        private readonly M3alamDbContext _context;

        public CreateRequestCommandHandler(M3alamDbContext context)
        {
            _context = context;
        }

        public async Task<Request> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
        {
            var newRequest = new Request
            {
                Id = Guid.NewGuid().ToString(),
                Title = request.Title,
                Description = request.Description,
                ServiceType = request.ServiceType,
                Status = request.Status,
                SeekerId = request.SeekerId,
                ProviderId = request.ProviderId,
            };

            _context.Requests.Add(newRequest);
            await _context.SaveChangesAsync(cancellationToken);

            return newRequest;
        }
    }
}
