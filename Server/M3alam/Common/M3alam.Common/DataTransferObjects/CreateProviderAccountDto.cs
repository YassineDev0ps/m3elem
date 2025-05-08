using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.DataTransferObjects
{
    public class CreateProviderAccountDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string? Bio { get; set; }
        public int Experience { get; set; }
        public string[]? Skills { get; set; }
        public string[]? Availability { get; set; }
       
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Address { get; set; }
    }
}
