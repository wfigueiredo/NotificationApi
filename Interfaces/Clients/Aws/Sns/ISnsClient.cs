using NotificationApi.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Clients.Aws.Sns
{
    public interface ISnsClient
    {
        Task PublishAsync(string TopicId, MessageDto payload);
    }
}
