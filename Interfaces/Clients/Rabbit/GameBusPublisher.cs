using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationApi.Domain.Model;
using NotificationApi.Infrastructure.Messaging;
using RabbitMQ.Client;

namespace NotificationApi.Interfaces.Clients
{
    public class GameBusPublisher : IBusPublisher, IDisposable
    {
        private const string ROUTING_KEY = "game";

        private IModel _channel;

        private string _exchange;
        private string _dlExchange;

        private readonly IBusConnection _busConnection;
        private readonly IConfiguration _config;
        private readonly ILogger<GameBusPublisher> _logger;

        public GameBusPublisher(IBusConnection busConnection, IConfiguration config, ILogger<GameBusPublisher> logger)
        {
            _busConnection = busConnection;
            _config = config;
            _logger = logger;
        }

        public void Start()
        {
            InitBusElements();
            InitChannel();
        }

        private void InitBusElements()
        {
            var rabbitMqConfig = _config.GetSection("RabbitMq");

            // exchanges
            _exchange = rabbitMqConfig["game:exchange"];
            _dlExchange = rabbitMqConfig["game:dlx"];
        }

        public void InitChannel()
        {
            _channel = _busConnection.CreateChannel();

            RegisterExchange(_exchange, ExchangeType.Topic);
            RegisterExchange(_dlExchange, ExchangeType.Topic);
        }

        private void RegisterExchange(string exchangeId, string exchangeType)
        {
            if (string.IsNullOrEmpty(exchangeId))
                throw new ArgumentNullException(nameof(exchangeId));

            _channel.ExchangeDeclare(exchange: exchangeId,
                                    type: exchangeType,
                                    durable: true);
        }

        /// <summary>
        /// Channel instance usage by more than one thread simultaneously should be avoided.
        /// Channel instances must not be shared by threads that publish on them.
        /// 
        /// The application MUST enforce mutual exclusion!
        /// 
        /// Link: https://www.rabbitmq.com/dotnet-api-guide.html#concurrency-channel-sharing
        /// 
        /// </summary>
        public void Enqueue(MessageDto message)
        {
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            try
            {
                lock (_channel)
                {
                    _channel.BasicPublish(exchange: _exchange,
                                            routingKey: ROUTING_KEY,
                                            basicProperties: null,
                                            body: body);

                    _logger.LogInformation($"Message Id {message.UniqueId} successfully published to RabbitMq exchange: {_exchange}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to publish Message Id {message.UniqueId} to RabbitMq exchange: {_exchange}. Reason: {ex.Message}");
                throw ex;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
