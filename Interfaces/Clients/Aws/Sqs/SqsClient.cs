using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NotificationApi.Domain.Model;
using SecretsManagerFacadeLib.Contracts;
using SecretsManagerFacadeLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Interfaces.Clients.Aws.Sqs
{
    public class SqsClient : ISqsClient
    {
        private readonly ILogger<SqsClient> _logger;
        private readonly ICredentialsFacade<AwsCredentials> _credentialsFacade;

        private AmazonSQSClient _client;

        public SqsClient(ICredentialsFacade<AwsCredentials> credentialsFacade, ILogger<SqsClient> logger)
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
            _client = new AmazonSQSClient(BasicAwsCredentials, Endpoint);

            _logger.LogInformation("Sqs Client created succesfully");
        }

        public async Task PublishAsync(string QueueId, MessageDto payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            var queueUrl = ComposeQueueUri(QueueId).AbsoluteUri;
            var messageBody = JsonConvert.SerializeObject(payload);
            var messageRequest = new SendMessageRequest(queueUrl, messageBody);

            if (queueUrl.EndsWith(".fifo"))
            {
                messageRequest.MessageGroupId = payload.GroupId;
            }

            try
            {
                await _client.SendMessageAsync(messageRequest);
                _logger.LogInformation($"Message Id {payload.UniqueId} successfully published to Sqs queue {queueUrl}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to publish Message Id {payload.UniqueId} to Sqs queue {queueUrl}");
                throw ex;
            }
        }

        private Uri ComposeQueueUri(string QueueName)
        {
            var awsCredentials = _credentialsFacade.GetCredentials();
            var Endpoint = RegionEndpoint.GetBySystemName(awsCredentials.Region).SystemName;
            return new Uri($"https://sqs.{Endpoint}.amazonaws.com/{awsCredentials.AccountId}/{QueueName}");
        }
    }
}
