using M3alam.Common.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.Interfaces
{

    public interface IMessageService
    {
        Task<IEnumerable<ChatThreadDto>> GetThreadsAsync(Guid userId);
        Task<IEnumerable<MessageDto>> GetMessagesAsync(Guid requestId);
        Task SendMessageAsync(Guid requestId, Guid senderId, CreateMessageDto dto);
    }
}
