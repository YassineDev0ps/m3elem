using MediatR;

namespace M3alam.Application.Commands
{
    public class CreateUserCommand : IRequest<User>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; } // e.g., Seeker, Provider
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
    }
}
