using Microsoft.Extensions.Logging;
using NotificationApi.Domain.Enum;
using NotificationApi.Domain.Model;
using NotificationApi.Interfaces.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Services
{
    public class RabbitPublisherService : IMessagePublisherService
    {
        private readonly ILogger<RabbitPublisherService> _logger;
        private readonly IBusPublisher _brokerClient;

        public RabbitPublisherService(ILogger<RabbitPublisherService> logger, IBusPublisher brokerClient)
        {
            _logger = logger;
            _brokerClient = brokerClient;
        }

        public async Task Publish(MessageDto message)
        {
            try
            {
                _brokerClient.Enqueue(message);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error publishing message {message.UniqueId} to destination");
                _logger.LogError($"Reason: {ex.Message}");
            }
        }
    }
}
