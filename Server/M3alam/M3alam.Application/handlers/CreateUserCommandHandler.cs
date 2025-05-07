using MediatR;
using M3alam.Infrastructure.Context;
using M3alam.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace M3alam.Application.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly M3alamDbContext _context;

        public CreateUserCommandHandler(M3alamDbContext context)
        {
            _context = context;
        }

        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Role = request.Role,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Address = request.Address,
                Token = "sample_token" // Placeholder token for now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return user;
        }
    }
}
