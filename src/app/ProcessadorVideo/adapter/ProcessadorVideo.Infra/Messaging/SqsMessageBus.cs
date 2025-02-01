using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using ProcessadorVideo.Domain.Adapters.MessageBus;
using ProcessadorVideo.Domain.DomainObjects;

namespace ProcessadorVideo.Infra.Messaging;

[ExcludeFromCodeCoverage]
public class SqsMessageBus : IMessageBus
{
    private readonly IAmazonSQS _client;

    public SqsMessageBus(IConfiguration configuration)
    {
        var awsConfig = configuration.GetSection("AWS");

        string serviceUrl = awsConfig["ServiceUrl"];
        string region = awsConfig["Region"] ?? "us-east-1";

        var sqsConfigClient = new AmazonSQSConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(region),
        };

        if (!string.IsNullOrEmpty(serviceUrl))
            sqsConfigClient.ServiceURL = serviceUrl;

        _client = new AmazonSQSClient(sqsConfigClient);
    }

    public async Task DeleteMessage(string topicOrQueue, string messageId)
    {
        try
        {
            await _client.DeleteMessageAsync(topicOrQueue, messageId);
        }
        catch (Exception ex)
        {
            throw new MessageBusException($"Erro ao deletar a mensagem do SQS: {ex.Message}");
        }
    }

    public async Task PublishAsync<T>(T message, string queueUrl)
    {
        try
        {
            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = message is string ? message.ToString() : JsonSerializer.Serialize(message)
            };

            await _client.SendMessageAsync(sendMessageRequest);
        }
        catch (Exception ex)
        {
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
            Console.WriteLine($"Erro ao obter a URL da fila {queueName}: {ex.Message}");
            throw;
        }
    }
}

