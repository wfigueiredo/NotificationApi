using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NotificationApi.Data;
using NotificationApi.Domain.Model;
using NotificationApi.Infrastructure.Extensions;
using NotificationApi.Infrastructure.HostedServices;
using NotificationApi.Infrastructure.Messaging;
using NotificationApi.Interfaces.Clients;
using NotificationApi.Interfaces.Clients.Aws.Sns;
using NotificationApi.Interfaces.Clients.Aws.Sqs;
using NotificationApi.Interfaces.Repository.Messages;
using NotificationApi.Interfaces.Services;
using NotificationApi.Interfaces.Services.Aws;
using NotificationApi.Interfaces.Services.MassTransit;
using NotificationApi.Interfaces.Services.MassTransit.Consumers;
using NotificationApi.Interfaces.Strategy;
using Npgsql;
using RabbitMQ.Client;
using SecretsManagerFacadeLib.Contracts;
using SecretsManagerFacadeLib.Interfaces;
using SecretsManagerFacadeLib.Interfaces.Clients;
using SecretsManagerFacadeLib.Interfaces.Clients.Impl;
using SecretsManagerFacadeLib.Interfaces.Impl;

namespace NotificationApi
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            // EF core
            services.AddDbContext<DataContext>(options =>
            {
                options.UseNpgsql(_config.GetConnectionString("PostgreSqlConnection"));
            });

            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.UseCamelCasing(true);
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                options.SerializerSettings.Converters.Add(new StringEnumConverter
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                });
            });

            // services (singleton)
            services.AddSingleton<PublishStrategyContext>();
            services.AddSingleton<RabbitPublisherService>();
            services.AddSingleton<SqsPublisherService>();
            services.AddSingleton<SnsPublisherService>();

            // services (scoped)
            services.AddScoped<IMessageService, MessageService>();  // VER SE PODE...!
            services.AddScoped<MassTransitPublisherService>();
            //services.AddScoped<IConsumer<MessageDto>, MessageDtoConsumer>();

            // repositories (scoped)
            services.AddScoped<IMessageRepository, MessageRepository>();
            
            // message broker
            services.AddSingleton<IBusConnection, BusConnection>();
            services.AddSingleton<IBusPublisher, GameBusPublisher>();
            services.AddSingleton<IBusSubscriber, GameBusSubscriber>();

            // Mass Transit
            services.AddMassTransit(options =>
            {
                var RabbitSection = _config.GetSection("RabbitMq");
                var GameBus = RabbitSection.GetSection("game");
                
                options.AddConsumer<MessageDtoConsumer>();
                options.UsingRabbitMq((provider, configurator) =>
                {
                    configurator.Host(RabbitSection["hostname"], RabbitSection["vhost"], credentials =>
                    {
                        credentials.Username(RabbitSection["username"]);
                        credentials.Password(RabbitSection["password"]);
                    });

                    // Producer settings
                    configurator.Send<MessageDto>(x => { x.UseRoutingKeyFormatter(context => context.Message.GroupId); }); // routing key
                    configurator.Message<MessageDto>(x => x.SetEntityName(GameBus["exchange"]));  // exchange
                    configurator.Publish<MessageDto>(x =>
                    {
                        x.ExchangeType = ExchangeType.Topic;
                    });

                    // Consumer settings
                    configurator.ReceiveEndpoint(GameBus.GetSection("queues")["playstation"], q =>
                    {
                        q.Durable = true;
                        q.Consumer<MessageDtoConsumer>(provider: provider);
                        q.Bind(GameBus["exchange"], s =>
                        {
                            s.RoutingKey = GameBus["routingKey"];
                            s.ExchangeType = ExchangeType.Topic;
                        });
                    });
                    configurator.ReceiveEndpoint(GameBus.GetSection("queues")["xbox"], q =>
                    {
                        q.Durable = true;
                        q.Consumer<MessageDtoConsumer>(provider: provider);
                        q.Bind(GameBus["exchange"], s =>
                        {
                            s.RoutingKey = GameBus["routingKey"];
                            s.ExchangeType = ExchangeType.Topic;
                        });
                    });
                });
            });

            services.AddMassTransitHostedService();   // automatically starts and stops the bus

            // clients
            services.AddSingleton(RegionEndpoint.SAEast1);
            services.AddSingleton<ISqsClient, SqsClient>();
            services.AddSingleton<ISnsClient, SnsClient>();
            services.AddSingleton<ISecretsManagerClient, SecretsManagerClient>();
            services.AddSingleton<ISecretsManagerFacade, SecretsManagerFacade>();
            services.AddSingleton<ICredentialsFacade<AwsCredentials>, AWSCredentialsFacade>();

            // hosted services
            //services.AddHostedService<GameBusBackgroundWorker>();    // bus bootstrap using vanilla RabbitMq client
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // extensions
            app.UseMessageLogMiddleware();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
