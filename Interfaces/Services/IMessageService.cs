using NotificationApi.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Services
{
    public interface IMessageService
    {
        Task CreateAsync(Message message);
        Task<IEnumerable<Message>> FindAllAsync();
        Task<Message> FindByUniqueIdAsync(string uniqueId);
    }
}
