using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.DataTransferObjects
{
    public class ProviderSearchDto : MatchCriteriaDto
    {
        public IEnumerable<string>? Skills { get; set; }
    }

}
