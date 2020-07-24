using Microsoft.EntityFrameworkCore;
using NotificationApi.Data;
using NotificationApi.Domain.Model;
using NotificationApi.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Repository.Messages
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _dataContext;

        public MessageRepository(DataContext context)
        {
            _dataContext = context;
        }

        public async Task CreateAsync(Message message)
        {
            await _dataContext.Messages.AddAsync(message);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> FindAllAsync()
        {
            return await _dataContext.Messages.ToListAsync();
        }

        public async Task<Message> FindByUniqueIdAsync(string uniqueId)
        {
            return await _dataContext.Messages
                .FirstOrDefaultAsync(message => message.UniqueId == uniqueId);
        }
    }
}
