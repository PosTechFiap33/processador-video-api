using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Adapters.Repositories;

namespace ProcessadorVideo.Infra.Messaging.Workers;

public class ProcessamentoVideoRealizadoMessagingWorker : MessagingWorker<ProcessamentoVideoRealizadoMessage>
{
    public ProcessamentoVideoRealizadoMessagingWorker(ILogger<MessagingWorker<ProcessamentoVideoRealizadoMessage>> logger,
                                                      IServiceProvider serviceProvider,
                                                      IOptions<AWSConfiguration> options)
                                                      : base(logger, serviceProvider, $"{options.Value.ConversaoVideoParaImagemRealizadaQueueUrl}")
    {
    }

    protected override async Task ProccessMessage(ProcessamentoVideoRealizadoMessage message, IServiceScope serviceScope)
    {
        try
        {
            var repository = serviceScope.ServiceProvider.GetService<IProcessamentoVideoRepository>();

            var processamento = await repository.Consultar(message.ProcessamentoId);

            processamento.Finalizar(message.Zip);

            await repository.UnitOfWork.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao atualizar o status do processamento! Id do procesamento: {message.ProcessamentoId}");
            throw;
        }
    }
}
