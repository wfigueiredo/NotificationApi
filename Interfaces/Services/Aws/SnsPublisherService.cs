using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationApi.Domain.Model;
using NotificationApi.Interfaces.Clients.Aws.Sns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Services.Aws
{
    public class SnsPublisherService : IMessagePublisherService
    {
        private readonly IConfiguration _config;
        private ISnsClient _snsClient;

        public SnsPublisherService(IConfiguration config, ISnsClient snsClient)
        {
            _config = config;
            _snsClient = snsClient;
        }

        public async Task Publish(MessageDto message)
        {
            var TopicId = GetTopicName(message.GroupId);
            await _snsClient.PublishAsync(TopicId, message);
        }

        private string GetTopicName(string GroupId) => GroupId switch
        {
            "game" => _config.GetValue<string>("Aws:Sns:game"),
            _ => throw new InvalidOperationException($"Unknown topic type")
        };
    }
}
