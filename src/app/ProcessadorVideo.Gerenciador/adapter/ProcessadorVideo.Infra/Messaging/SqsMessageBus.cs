using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.Domain.Adapters.MessageBus;
using ProcessadorVideo.Domain.DomainObjects;

namespace ProcessadorVideo.Infra.Messaging;

[ExcludeFromCodeCoverage]
public class SqsMessageBus : IMessageBus
{
    private readonly IAmazonSQS _client;
    private readonly ILogger<SqsMessageBus> _logger;

    public SqsMessageBus(IOptions<AWSConfiguration> configuration,
                         ILogger<SqsMessageBus> logger)
    {
        try
        {
            _logger = logger;

            var awsConfig = configuration.Value;

            var sqsConfigClient = new AmazonSQSConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsConfig.Region),
            };

            if (string.IsNullOrEmpty(awsConfig.ServiceUrl))
            {
                var credentials = new SessionAWSCredentials(awsConfig.AccesKey, awsConfig.Secret, awsConfig.Token);
                _client = new AmazonSQSClient(credentials, sqsConfigClient);
            }
            else
            {
                sqsConfigClient.ServiceURL = awsConfig.ServiceUrl;
                _client = new AmazonSQSClient(sqsConfigClient);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao criar as credencias da aws: {ex.Message}");
            throw new MessageBusException("Ocorreu um erro ao autenticar com o provedor de cloud!");
        }
    }

    public async Task DeleteMessage(string queueName, string messageId)
    {
        try
        {
            var queueUrl = await GetQueueUrlAsync(queueName);
            await _client.DeleteMessageAsync(queueUrl, messageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro remover a mensagem da fila: {ex.Message}");
            throw new MessageBusException($"Erro ao deletar a mensagem do SQS: {ex.Message}");
        }
    }

    public async Task PublishAsync<T>(T message, string queueName)
    {
        try
        {
            var queueUrl = await GetQueueUrlAsync(queueName);

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = message is string ? message.ToString() : JsonSerializer.Serialize(message)
            };

            await _client.SendMessageAsync(sendMessageRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro publicar a mensagem na fila: {ex.Message}");
            throw new MessageBusException($"Erro ao publicar a mensagem do SQS: {ex.Message}");
        }
    }

    public async Task<IEnumerable<MessageResult<T>>> ReceiveMessagesAsync<T>(string queueName, int maxMessages = 10, int waitTimeSeconds = 5, int visibilityTimeout = 20)
    {
        try
        {
            var queue = await GetQueueUrlAsync(queueName);

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queue,
                MaxNumberOfMessages = maxMessages,
                WaitTimeSeconds = waitTimeSeconds,
                VisibilityTimeout = visibilityTimeout,
                AttributeNames = new List<string> { "All" }, // Retorna todos os atributos da mensagem
                MessageAttributeNames = new List<string> { "All" } // Retorna todos os atributos customizados da mensagem
            };

            var response = await _client.ReceiveMessageAsync(receiveMessageRequest);

            var messages = new List<MessageResult<T>>();

            foreach (var message in response.Messages)
                messages.Add(new MessageResult<T>(JsonSerializer.Deserialize<T>(message.Body, new JsonSerializerOptions
                {
                    IncludeFields = true
                }), message.ReceiptHandle));

            return messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro receber a mensagem na fila: {ex.Message}");
            throw new MessageBusException($"Erro ao receber a mensagem do SQS: {ex.Message}");
        }
    }

    public async Task<string> GetQueueUrlAsync(string queueName)
    {
        try
        {
            var request = new GetQueueUrlRequest
            {
                QueueName = queueName
            };

            var response = await _client.GetQueueUrlAsync(request);

            return response.QueueUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao recuperar a url da fila: {ex.Message}");
            throw new MessageBusException($"Erro ao recuperar nome da fila SQS: {ex.Message}");
        }
    }
}

