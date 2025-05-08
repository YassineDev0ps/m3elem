using M3alam.Common.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.Interfaces
{
    public interface IRatingService
    {
        Task<RatingDto> GetRatingAsync(Guid requestId);
        Task AddOrUpdateRatingAsync(Guid requestId, RatingDto dto);
    }

}
