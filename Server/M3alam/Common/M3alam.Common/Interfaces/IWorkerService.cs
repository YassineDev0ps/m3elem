using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.Interfaces
{
    public interface IWorkerService
    {
        Task<string> SendAsync(string message);
    }
}
