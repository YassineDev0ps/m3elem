using Azure.Core;
using M3alam.Identity.Infrastructure.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Domain.Entities
{
    public class Provider : ApplicationUser
    {
        public string? Bio { get; set; }
        public int Experience { get; set; }
        public string[]? Skills { get; set; }
        public string[]? Availability { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public int CompletedJobs { get; set; }

        [JsonIgnore]
        public ICollection<Request>? RequestsHandled { get; set; }

        [JsonIgnore]
        public ICollection<Rating>? RatingsReceived { get; set; }
    }

}
