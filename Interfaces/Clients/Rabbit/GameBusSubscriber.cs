using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationApi.Domain.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationApi.Infrastructure.Messaging
{
    public class GameBusSubscriber : IBusSubscriber, IDisposable
    {
        private const string ROUTING_KEY = "game";
        private const int MAX_CONSUMERS = 2;

        private IModel _channel;

        private int SuccessfulDeliveries;
        
        private string _exchange;
        private string _dlExchange;
        private string _ps5QueueId;
        private string _seriesxQueueId;

        private readonly IBusConnection _busConnection;
        private readonly IConfiguration _config;
        private readonly ILogger<GameBusSubscriber> _logger;

        public GameBusSubscriber(IBusConnection busConnection, IConfiguration config, ILogger<GameBusSubscriber> logger)
        {
            _busConnection = busConnection;
            _config = config;
            _logger = logger;
        }

        public void Start()
        {
            InitBusElements();
            InitChannel();
            InitSubscription();
        }

        private void InitBusElements()
        {
            var rabbitMqConfig = _config.GetSection("RabbitMq");

            // exchanges
            _exchange = rabbitMqConfig["game:exchange"];
            _dlExchange = rabbitMqConfig["game:dlx"];

            // queues
            _ps5QueueId = rabbitMqConfig["game:queues:playstation"];
            _seriesxQueueId = rabbitMqConfig["game:queues:xbox"];
        }

        private void InitChannel()
        {
            _channel = _busConnection.CreateChannel();

            RegisterExchange(_exchange, ExchangeType.Topic);
            RegisterExchange(_dlExchange, ExchangeType.Topic);

            RegisterQueue(_ps5QueueId, _exchange);
            RegisterQueue(_seriesxQueueId, _exchange);
        }

        private void RegisterExchange(string exchangeId, string exchangeType)
        {
            if (string.IsNullOrEmpty(exchangeId))
                throw new ArgumentNullException(nameof(exchangeId));

            _channel.ExchangeDeclare(exchange: exchangeId,
                                    type: exchangeType,
                                    durable: true);
        }

        private void RegisterQueue(string queueId, string exchangeId)
        {
            if (string.IsNullOrEmpty(queueId)) 
                throw new ArgumentNullException(nameof(queueId));

            var queueDeclare = _channel.QueueDeclare(
                                queue: queueId,
                                durable: true,
                                autoDelete: false);

            _channel.QueueBind(queue: queueDeclare,
                              exchange: exchangeId,
                              routingKey: ROUTING_KEY);
        }

        private void InitSubscription()
        {
            Enumerable.Range(1, MAX_CONSUMERS)
                .Select(i => new AsyncEventingBasicConsumer(_channel))
                .ToList()
                .ForEach(cs =>
                {
                    _channel.BasicConsume(queue: _ps5QueueId, autoAck: false, consumer: cs);
                    _channel.BasicConsume(queue: _seriesxQueueId, autoAck: false, consumer: cs);
                    cs.Received += OnMsgReceivedAsync;
                });
        }

        private async Task OnMsgReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            var body = eventArgs.Body.ToArray();
            var rawMessage = Encoding.UTF8.GetString(body);
            var message = JsonConvert.DeserializeObject<Message>(rawMessage);
            _channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);

            SuccessfulDeliveries++;
            _logger.LogInformation($"Successful delivery count: {SuccessfulDeliveries}");

            await Task.Yield();
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
