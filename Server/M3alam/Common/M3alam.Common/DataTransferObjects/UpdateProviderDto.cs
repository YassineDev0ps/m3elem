using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.DataTransferObjects
{
    public class UpdateProviderDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Bio { get; set; }
        public int? Experience { get; set; }
        public IEnumerable<string>? Skills { get; set; }
        public IEnumerable<string>? Availability { get; set; }
    }
}
