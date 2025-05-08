using M3alam.Common.Entities;
using M3alam.Common.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Domain.Entities
{
    public class Request : BaseEntity<Guid>
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string ServiceType { get; set; } = default!;
        public RequestStatusEnum Status { get; set; }
        public DateTime PreferredTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; } = default!;
        // Foreign keys
        public Guid SeekerId { get; set; }
        public Guid? ProviderId { get; set; }

       
   
        public ICollection<Message>? Messages { get; set; }
    }
}
