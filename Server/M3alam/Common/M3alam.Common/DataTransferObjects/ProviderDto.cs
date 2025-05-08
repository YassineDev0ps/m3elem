using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.DataTransferObjects
{
    public class ProviderDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string? Bio { get; set; }
        public double Rating { get; set; }
        public double DistanceKm { get; set; }
    }

}
