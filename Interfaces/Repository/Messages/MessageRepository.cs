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
        private readonly DataContext _context;

        public MessageRepository(DataContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> FindAllAsync()
        {
            return await _context.Messages.ToListAsync();
        }

        public async Task<Message> FindByUniqueIdAsync(string uniqueId)
        {
            return await _context.Messages
                .FirstOrDefaultAsync(message => message.UniqueId == uniqueId);
        }
    }
}
