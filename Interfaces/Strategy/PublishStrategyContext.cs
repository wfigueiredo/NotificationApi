using NotificationApi.Domain.Enum;
using NotificationApi.Domain.Model;
using NotificationApi.Interfaces.Services;
using NotificationApi.Interfaces.Services.Aws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Strategy
{
    public class PublishStrategyContext
    {
        private readonly RabbitPublisherService _rabbitPublisher;
        private readonly SqsPublisherService _sqsPublisher;
        private readonly SnsPublisherService _snsPublisher;

        public PublishStrategyContext(
            RabbitPublisherService rabbitPublisher,
            SqsPublisherService sqsPublisher,
            SnsPublisherService snsPublisher)
        {
            _rabbitPublisher = rabbitPublisher;
            _sqsPublisher = sqsPublisher;
            _snsPublisher = snsPublisher;
        }

        public Task Apply(MessageDto message) => message.Metadata.Destination switch
        {
            DestinationType.Sqs => _sqsPublisher.Publish(message),
            DestinationType.Sns => _snsPublisher.Publish(message),
            DestinationType.Rabbit => _rabbitPublisher.Publish(message),
            _ => throw new InvalidOperationException($"Unknown destination type")
        };
    }
}
