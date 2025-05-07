using MediatR;

namespace M3alam.Application.Commands
{
    public class CreateProviderCommand : IRequest<Provider>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Bio { get; set; }
        public string Skills { get; set; }
        public string Availability { get; set; }
        public int Experience { get; set; }
    }
}
