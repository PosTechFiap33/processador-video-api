using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.Domain.Adapters.MessageBus;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Adapters.Repositories;

namespace ProcessadorVideo.Infra.Messaging.Workers;

public class ProcessamentoVideoRealizadoMessagingWorker : MessagingWorker<ProcessamentoVideoRealizadoMessage>
{
    private readonly AWSConfiguration _awsConfiguration;

    public ProcessamentoVideoRealizadoMessagingWorker(ILogger<MessagingWorker<ProcessamentoVideoRealizadoMessage>> logger,
                                                      IServiceProvider serviceProvider,
                                                      IOptions<AWSConfiguration> options)
                                                      : base(logger, serviceProvider, $"{options.Value.ConversaoVideoParaImagemRealizadaQueueUrl}",
                                                      20)
    {
        _awsConfiguration = options.Value;
    }

    protected override async Task ProccessMessage(ProcessamentoVideoRealizadoMessage message, IServiceScope serviceScope)
    {
        var _messageBus = serviceScope.ServiceProvider.GetRequiredService<IMessageBus>();

        try
        {
            var repository = serviceScope.ServiceProvider.GetService<IProcessamentoVideoRepository>();

            var processamento = await repository.Consultar(message.ProcessamentoId);

            processamento.Finalizar(message.Zip);

            await repository.Atualizar(processamento);
        }
        catch (Exception ex)
        {
            var erroProcessamentoVideoMessage = new ErroProcessamentoVideoMessage(message.ProcessamentoId, ex.Message);
            await _messageBus.PublishAsync(erroProcessamentoVideoMessage, _awsConfiguration.ConversaoVideoParaImagemErroQueueUrl);
            _logger.LogError(ex, $"Ocorreu um erro ao atualizar o status do processamento! Id do procesamento: {message.ProcessamentoId}");
        }
    }
}
