using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Adapters.Repositories;

namespace ProcessadorVideo.Infra.Messaging.Workers;

public class ProcessamentoVideoErrorMessagingWorker : MessagingWorker<ErroProcessamentoVideoMessage>
{
    public ProcessamentoVideoErrorMessagingWorker(ILogger<ProcessamentoVideoErrorMessagingWorker> logger,
                                                  IServiceProvider serviceProvider,
                                                  IOptions<AWSConfiguration> options) :
                                                  base(logger, serviceProvider, options.Value.ConversaoVideoParaImagemErroQueueUrl, 120)
    {
    }

    protected override async Task ProccessMessage(ErroProcessamentoVideoMessage message, IServiceScope scope)
    {
        try
        {
            var repository = scope.ServiceProvider.GetService<IProcessamentoVideoRepository>();

            var processamento = await repository.Consultar(message.ProcessamentoId);

            processamento.AdicionarErroProcessamento(message.Erro);

            await repository.Atualizar(processamento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao processar a mensagem de erro na fila: {ex.Message}");
        }
    }
}
