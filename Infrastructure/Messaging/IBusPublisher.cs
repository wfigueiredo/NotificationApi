﻿using NotificationApi.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Clients
{
    public interface IBusPublisher
    {
        void Start();
        void Enqueue(MessageDto message);
    }
}
