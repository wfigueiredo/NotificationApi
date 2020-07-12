using NotificationApi.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Repository.Messages
{
    public interface IMessageRepository
    {
        Task CreateAsync(Message message);
        Task<Message> FindByUniqueIdAsync(string uniqueId);
        Task<IEnumerable<Message>> FindAllAsync();
    }
}
