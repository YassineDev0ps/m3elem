using M3alam.Common.Entities;
using M3alam.Identity.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Domain.Entities
{
    public class Message : BaseEntity<Guid>
    {
        public Guid RequestId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = default!;
        public DateTime Timestamp { get; set; }

        // Navigation
        public Request? Request { get; set; }
        public ApplicationUser? Sender { get; set; }
    }
}
