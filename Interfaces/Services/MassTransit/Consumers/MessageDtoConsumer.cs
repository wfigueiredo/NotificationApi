using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationApi.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Services.MassTransit.Consumers
{
    public class MessageDtoConsumer : IConsumer<MessageDto>
    {
        private ILogger<MessageDtoConsumer> _logger;

        public MessageDtoConsumer(ILogger<MessageDtoConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MessageDto> context)
        {
            _logger.LogInformation($"UUID: {context.Message.UniqueId}");
            _logger.LogInformation($"Content: {context.Message.Content}");
            await Task.CompletedTask;
        }
    }
}
