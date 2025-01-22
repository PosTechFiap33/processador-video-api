namespace ProcessadorVideo.Domain.Adapters.MessageBus.Messages;

public class ErroProcessamentoVideoMessage
{
    public Guid ProcessamentoId { get; private set; }
    public string Erro { get; private set; }

    public ErroProcessamentoVideoMessage(Guid processamentoId, string erro)
    {
        ProcessamentoId = processamentoId;
        Erro = erro;
    }
}
