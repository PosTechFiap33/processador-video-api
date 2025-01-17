using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.CrossCutting.Factories;
using ProcessadorVideo.Domain.Adapters.MessageBus;
using ProcessadorVideo.Domain.DomainObjects;

namespace ProcessadorVideo.Infra.Messaging;

[ExcludeFromCodeCoverage]
public class SqsMessageBus : IMessageBus
{
    private readonly AWSConfiguration _configuration;

    public SqsMessageBus(IOptions<AWSConfiguration> aWSConfiguration)
    {
        _configuration = aWSConfiguration.Value;
    }

    public async Task DeleteMessage(string topicOrQueue, string messageId)
    {
        try
        {
            var client = CreateClient();

            await client.DeleteMessageAsync(topicOrQueue, messageId);
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
            var client = CreateClient();

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = message is string ? message.ToString() : JsonSerializer.Serialize(message)
            };

            await client.SendMessageAsync(sendMessageRequest);
        }
        catch (Exception ex)
        {
            throw new MessageBusException($"Erro ao publicar a mensagem do SQS: {ex.Message}");
        }
    }

    public async Task<IEnumerable<MessageResult<T>>> ReceiveMessagesAsync<T>(string queueUrl, int maxMessages = 10, int waitTimeSeconds = 5)
    {
        try
        {
            var client = CreateClient();

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = maxMessages,
                WaitTimeSeconds = waitTimeSeconds,
                AttributeNames = new List<string> { "All" }, // Retorna todos os atributos da mensagem
                MessageAttributeNames = new List<string> { "All" } // Retorna todos os atributos customizados da mensagem
            };

            var response = await client.ReceiveMessageAsync(receiveMessageRequest);

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

    private AmazonSQSClient CreateClient()
    {
        var config = new AmazonSQSConfig();

        if (!string.IsNullOrEmpty(_configuration.ServiceUrl))
            config.ServiceURL = _configuration.ServiceUrl;

        return new AmazonSQSClient(config.CreateCredentials(_configuration), config);
    }
}

