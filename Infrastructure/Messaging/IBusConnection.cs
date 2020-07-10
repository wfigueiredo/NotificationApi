using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Infrastructure.Messaging
{
    public interface IBusConnection
    {
        bool IsConnected { get; }
        IModel CreateChannel();
    }
}
