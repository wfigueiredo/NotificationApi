using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace NotificationApi.Infrastructure.Messaging
{
    public class BusConnection : IBusConnection, IDisposable
    {
        private readonly IConfiguration _config;
        private IConnection _connection;
        private bool _disposed;

        private readonly object semaphore = new object();

        public BusConnection(IConfiguration config)
        {
            _config = config;
        }

        public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

        
        private void TryConnect()
        {
            lock (semaphore)
            {
                if (IsConnected)
                    return;

                _connection = BuildConnection().CreateConnection();
                _connection.ConnectionShutdown += (s, e) => TryConnect();
                _connection.CallbackException += (s, e) => TryConnect();
                _connection.ConnectionBlocked += (s, e) => TryConnect();
            }
        }

        public IModel CreateChannel()
        {
            TryConnect();
            return _connection.CreateModel();
        }

        private ConnectionFactory BuildConnection()
        {
            var rabbitMqConfig = _config.GetSection("RabbitMq");
            return new ConnectionFactory()
            {
                HostName = rabbitMqConfig["hostname"],
                Port = Convert.ToInt32(rabbitMqConfig["port"]),
                VirtualHost = rabbitMqConfig["vhost"],
                UserName = rabbitMqConfig["username"],
                Password = rabbitMqConfig["password"],
                AutomaticRecoveryEnabled = true,
                DispatchConsumersAsync = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _connection?.Dispose();
        }
    }
}
