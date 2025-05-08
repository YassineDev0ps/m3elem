using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.DataTransferObjects
{
    public class ProviderProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string? Bio { get; set; }
        public int Experience { get; set; }
        public IEnumerable<string>? Skills { get; set; }
        public IEnumerable<string>? Availability { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public int CompletedJobs { get; set; }
    }
}
