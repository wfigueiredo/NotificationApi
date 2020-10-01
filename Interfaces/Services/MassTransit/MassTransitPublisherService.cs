using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using NotificationApi.Domain.Model;

namespace NotificationApi.Interfaces.Services.MassTransit
{
    public class MassTransitPublisherService : IMessagePublisherService
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitPublisherService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Publish(MessageDto message)
        {
            await _publishEndpoint.Publish(message);
        }
    }
}
