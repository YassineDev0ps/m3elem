using MediatR;

namespace M3alam.Application.Commands
{
    public class CreateSeekerCommand : IRequest<Seeker>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
