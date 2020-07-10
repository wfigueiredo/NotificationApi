using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationApi.Domain.Dto;
using NotificationApi.Domain.Enum;
using NotificationApi.Domain.Model;
using NotificationApi.Infrastructure.Extensions;
using NotificationApi.Interfaces.Clients.Aws.Sqs;

namespace NotificationApi.Interfaces.Services.Aws
{
    public class SqsPublisherService : IMessagePublisherService
    {
        private readonly IConfiguration _config;
        private ISqsClient _sqsClient;

        public SqsPublisherService(IConfiguration config, ISqsClient sqsClient)
        {
            _config = config;
            _sqsClient = sqsClient;
        }

        public async Task Publish(MessageDto message)
        {
            var queueId = GetQueueName(message.GroupId, message.Metadata.ComponentId);
            await _sqsClient.PublishAsync(queueId, message);
        }

        private string GetQueueName(string GroupId, string ComponentId) => GroupId switch
        {
            "game" => GetGameQueueId(Enum.Parse<ConsoleType>(ComponentId, true)),
            _ => throw new InvalidOperationException($"Unknown entity type")
        };

        private string GetGameQueueId(ConsoleType consoleType) => consoleType switch
        {
            ConsoleType.Playstation => _config.GetValue<string>("Aws:Sqs:game:queues:playstation"),
            ConsoleType.XBox => _config.GetValue<string>("Aws:Sqs:game:queues:xbox"),
            ConsoleType.Switch => _config.GetValue<string>("Aws:Sqs:game:queues:switch"),
            _ => throw new InvalidOperationException($"Unknown game queue type")
        };
    }
}
