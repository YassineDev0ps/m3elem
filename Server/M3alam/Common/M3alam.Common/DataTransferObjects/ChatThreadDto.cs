using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.DataTransferObjects
{
    public class ChatThreadDto
    {
        public Guid RequestId { get; set; }
        public string LastMessage { get; set; } = default!;
        public DateTime LastTimestamp { get; set; }
        public ProviderDto? Provider { get; set; }
        public SeekerProfileDto? Seeker { get; set; }
    }
}
