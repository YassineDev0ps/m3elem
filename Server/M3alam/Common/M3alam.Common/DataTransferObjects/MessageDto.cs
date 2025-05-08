using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.DataTransferObjects
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        public string SenderName { get; set; } = default!;
    }

}
