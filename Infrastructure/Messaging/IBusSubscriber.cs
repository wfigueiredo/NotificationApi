using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Infrastructure.Messaging
{
    public interface IBusSubscriber
    {
        void Start();
    }
}
