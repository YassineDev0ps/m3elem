using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.DataTransferObjects
{
    public class RequestDetailsDto : RequestDto
    {
        public SeekerProfileDto Seeker { get; set; } = default!;
        public ProviderProfileDto? Provider { get; set; }
        public IEnumerable<MessageDto>? Messages { get; set; }
        public RatingDto? Rating { get; set; }
    }
}
