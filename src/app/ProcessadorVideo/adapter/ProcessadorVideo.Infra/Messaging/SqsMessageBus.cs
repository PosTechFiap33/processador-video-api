using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using ProcessadorVideo.Domain.Adapters.MessageBus;
using ProcessadorVideo.Domain.DomainObjects;

namespace ProcessadorVideo.Infra.Messaging;

[ExcludeFromCodeCoverage]
public class SqsMessageBus : IMessageBus
{
    private readonly IAmazonSQS _client;

    public SqsMessageBus(IAmazonSQS sqsClient)
    {
        _client = sqsClient;
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

    public async Task<IEnumerable<MessageResult<T>>> ReceiveMessagesAsync<T>(string queueUrl, int maxMessages = 10, int waitTimeSeconds = 5, int visibilityTimeout = 20)
    {
        try
        {
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
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
}

