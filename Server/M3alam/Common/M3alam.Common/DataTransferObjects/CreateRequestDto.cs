using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.DataTransferObjects
{
    public class CreateRequestDto
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string ServiceType { get; set; } = default!;
        public DateTime PreferredTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; } = default!;
    }

}
