using NotificationApi.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Clients.Aws.Sqs
{
    public interface ISqsClient
    {
        Task PublishAsync(string QueueId, MessageDto payload);
    }
}
