using M3alam.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Domain.Entities
{
    public class Rating : BaseEntity<Guid>
    {
        public Guid RequestId { get; set; }
        public Guid ProviderId { get; set; }
        public Guid SeekerId { get; set; }
        public int Score { get; set; }     // renamed from ‘rating’
        public string? Comment { get; set; }

        // Navigation
        public Request? Request { get; set; }
        public Provider? Provider { get; set; }
        public Seeker? Seeker { get; set; }
    }
}
