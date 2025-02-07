using System;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Infra.Messaging.Workers;

namespace ProcessadorVideo.Gerenciador.Api.Consumers;


public class ProcessamentoVideoErroConsumer : MessagingWorker<ErroProcessamentoVideoMessage>
{
    public ProcessamentoVideoErroConsumer(ILogger<ProcessamentoVideoErroConsumer> logger,
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
