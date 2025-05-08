using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.DataTransferObjects
{
    public class DashboardDto
    {
        public int PendingRequests { get; set; }
        public int ActiveRequests { get; set; }
        public int CompletedRequests { get; set; }
        public double AverageRating { get; set; }
    }
}
