using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProcessadorVideo.Domain.Adapters.MessageBus;

namespace ProcessadorVideo.Infra.Messaging.Workers;


public abstract class MessagingWorker<T> : BackgroundService
{
    protected readonly IServiceProvider _serviceProvider;
    protected readonly ILogger<MessagingWorker<T>> _logger;
    private readonly string _queueUrl;
    private readonly int _visibilityTimeOut;
    protected abstract Task ProccessMessage(T message, IServiceScope serviceScope);


    public MessagingWorker(ILogger<MessagingWorker<T>> logger,
                           IServiceProvider serviceProvider,
                           string queueUrl,
                           int visibilityTimeout /*TODO: criar arquivo de config e definir instancia default para essas configs*/)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _queueUrl = queueUrl;
        _serviceProvider = serviceProvider;
        _visibilityTimeOut = visibilityTimeout;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando observação da fila SQS...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

                    var messages = await messageBus.ReceiveMessagesAsync<T>(_queueUrl, visibilityTimeout: _visibilityTimeOut);

                    foreach (var message in messages)
                    {
                        await ProccessMessage(message.data, scope);

                        _logger.LogInformation($"Mensagem recebida: {message}");

                        await messageBus.DeleteMessage(_queueUrl, message.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"queue: {_queueUrl} - Erro ao processar mensagens da fila. {ex.Message}");
                _logger.LogError(ex, "Erro ao processar mensagens da fila.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}