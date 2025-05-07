using MediatR;
using M3alam.Infrastructure.Context;
using M3alam.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace M3alam.Application.Handlers
{
    public class CreateProviderCommandHandler : IRequestHandler<CreateProviderCommand, Provider>
    {
        private readonly M3alamDbContext _context;

        public CreateProviderCommandHandler(M3alamDbContext context)
        {
            _context = context;
        }

        public async Task<Provider> Handle(CreateProviderCommand request, CancellationToken cancellationToken)
        {
            var provider = new Provider
            {
                Id = Guid.NewGuid().ToString(),
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Bio = request.Bio,
                Skills = request.Skills,
                Availability = request.Availability,
                Experience = request.Experience,
            };

            _context.Providers.Add(provider);
            await _context.SaveChangesAsync(cancellationToken);

            return provider;
        }
    }
}
