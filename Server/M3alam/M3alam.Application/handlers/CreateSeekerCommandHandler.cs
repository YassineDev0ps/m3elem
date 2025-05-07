using MediatR;
using M3alam.Infrastructure.Context;
using M3alam.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace M3alam.Application.Handlers
{
    public class CreateSeekerCommandHandler : IRequestHandler<CreateSeekerCommand, Seeker>
    {
        private readonly M3alamDbContext _context;

        public CreateSeekerCommandHandler(M3alamDbContext context)
        {
            _context = context;
        }

        public async Task<Seeker> Handle(CreateSeekerCommand request, CancellationToken cancellationToken)
        {
            var seeker = new Seeker
            {
                Id = Guid.NewGuid().ToString(),
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
            };

            _context.Seekers.Add(seeker);
            await _context.SaveChangesAsync(cancellationToken);

            return seeker;
        }
    }
}
