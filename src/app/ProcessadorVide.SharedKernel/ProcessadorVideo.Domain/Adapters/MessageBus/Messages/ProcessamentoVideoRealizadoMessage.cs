namespace ProcessadorVideo.Domain.Adapters.MessageBus.Messages;

public class ProcessamentoVideoRealizadoMessage
{
    public Guid ProcessamentoId { get; private set; }
    public Arquivo Zip { get; private set; }

    public ProcessamentoVideoRealizadoMessage(Guid processamentoId, Arquivo zip)
    {
        ProcessamentoId = processamentoId;
        Zip = zip;
    }
}
