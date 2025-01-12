using System.Diagnostics.CodeAnalysis;

namespace ProcessadorVideo.Domain.Adapters.MessageBus.Messages;

[ExcludeFromCodeCoverage]
public class ProcessarVideoMessage
{
    public Guid ProcessamentoId { get; set; }
    public string DiretorioVideos { get; set; }

    public ProcessarVideoMessage(Guid processamentoId, string diretorio)
    {
        ProcessamentoId = processamentoId;
        DiretorioVideos = diretorio;
    }
}
