using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationApi.Domain.Model;
using NotificationApi.Util;
using SecretsManagerFacadeLib.Contracts;
using SecretsManagerFacadeLib.Interfaces;

namespace NotificationApi.Interfaces.Clients.Aws.Sns
{
    public class SnsClient : ISnsClient
    {
        private readonly ILogger<SnsClient> _logger;
        private readonly ICredentialsFacade<AwsCredentials> _credentialsFacade;
        private AmazonSimpleNotificationServiceClient _client;

        public SnsClient(ICredentialsFacade<AwsCredentials> credentialsFacade, ILogger<SnsClient> logger)
        {
            _logger = logger;
            _credentialsFacade = credentialsFacade;
            InitBus();
        }

        private void InitBus()
        {
            var awsCredentials = _credentialsFacade.GetCredentials();
            var BasicAwsCredentials = new BasicAWSCredentials(awsCredentials.AccessKey, awsCredentials.SecretKey);
            var Endpoint = RegionEndpoint.GetBySystemName(awsCredentials.Region);
            _client = new AmazonSimpleNotificationServiceClient(BasicAwsCredentials, Endpoint);

            _logger.LogInformation("Sns client created succesfully");
        }

        private async Task<string> GetTopicArnAsync(string topicName)
        {
            var TopicInfo = await _client.FindTopicAsync(topicName);
            return TopicInfo.TopicArn;
        }

        public async Task PublishAsync(string TopicId, MessageDto payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            var TopicArn = await GetTopicArnAsync(TopicId);
            var MessageBody = JsonConvert.SerializeObject(payload);
            var result = await _client.PublishAsync(TopicArn, MessageBody);

            if (!HttpUtil.IsSuccessStatusCode(result.HttpStatusCode))
            {
                _logger.LogError($"Could not publish to Sns topic {TopicId}");
                throw new Exception($"Could not publish to {TopicId}");
            }
            else
            {
                _logger.LogInformation($"Message Id {payload.UniqueId} successfully published to Sns topic {TopicArn}");
            }
        }
    }
}
