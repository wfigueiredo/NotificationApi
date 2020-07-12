using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotificationApi.Data;
using NotificationApi.Domain.Model;
using NotificationApi.Interfaces.Repository.Messages;

namespace NotificationApi.Interfaces.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;

        public MessageService(IMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateAsync(Message message)
        {
            await _repository.CreateAsync(message);
        }

        public async Task<IEnumerable<Message>> FindAllAsync()
        {
            return await _repository.FindAllAsync();
        }

        public async Task<Message> FindByUniqueIdAsync(string uniqueId)
        {
            return await _repository.FindByUniqueIdAsync(uniqueId);
        }
    }
}
