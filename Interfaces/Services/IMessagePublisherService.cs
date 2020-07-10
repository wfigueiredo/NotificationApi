using NotificationApi.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Services
{
    public interface IMessagePublisherService
    {
        Task Publish(MessageDto message);
    }
}
