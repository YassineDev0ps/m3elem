using MediatR;

namespace M3alam.Application.Commands
{
    public class CreateRequestCommand : IRequest<Request>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ServiceType { get; set; }
        public string Status { get; set; }
        public string SeekerId { get; set; }
        public string ProviderId { get; set; }
    }
}
