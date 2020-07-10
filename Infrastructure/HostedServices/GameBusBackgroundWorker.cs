using Microsoft.Extensions.Hosting;
using NotificationApi.Infrastructure.Messaging;
using NotificationApi.Interfaces.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationApi.Infrastructure.HostedServices
{
    public class GameBusBackgroundWorker : BackgroundService
    {
        private readonly IBusPublisher _publisher;
        private readonly IBusSubscriber _subscriber;

        public GameBusBackgroundWorker(IBusPublisher publisher, IBusSubscriber subscriber)
        {
            _publisher = publisher;
            _subscriber = subscriber;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _publisher.Start();
            _subscriber.Start();

            await Task.CompletedTask;
        }
    }
}
