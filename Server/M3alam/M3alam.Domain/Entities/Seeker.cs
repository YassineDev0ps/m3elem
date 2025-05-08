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
    public class Seeker : ApplicationUser
    {
       
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Address { get; set; }

        [JsonIgnore]
        public ICollection<Request>? Requests { get; set; }

        [JsonIgnore]
        public ICollection<Rating>? RatingsGiven { get; set; }
    }
}
