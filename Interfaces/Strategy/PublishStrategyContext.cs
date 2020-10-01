using NotificationApi.Domain.Enum;
using NotificationApi.Domain.Model;
using NotificationApi.Interfaces.Services;
using NotificationApi.Interfaces.Services.Aws;
using NotificationApi.Interfaces.Services.MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Strategy
{
    public class PublishStrategyContext
    {
        private readonly RabbitPublisherService _rabbitPublisher;
        private readonly MassTransitPublisherService _massTransitPublisher;
        private readonly SqsPublisherService _sqsPublisher;
        private readonly SnsPublisherService _snsPublisher;

        public PublishStrategyContext(
            RabbitPublisherService rabbitPublisher,
            MassTransitPublisherService massTransitPublisher,
            SqsPublisherService sqsPublisher,
            SnsPublisherService snsPublisher)
        {
            _rabbitPublisher = rabbitPublisher;
            _massTransitPublisher = massTransitPublisher;
            _sqsPublisher = sqsPublisher;
            _snsPublisher = snsPublisher;
        }

        public Task Apply(MessageDto message) => message.Metadata.channel switch
        {
            ChannelType.Sqs => _sqsPublisher.Publish(message),
            ChannelType.Sns => _snsPublisher.Publish(message),
            ChannelType.MassTransit => _massTransitPublisher.Publish(message),
            ChannelType.Rabbit => _rabbitPublisher.Publish(message),
            _ => throw new InvalidOperationException($"Unknown destination type")
        };
    }
}
