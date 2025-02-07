using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.DomainObjects;

namespace ProcessadorVideo.Domain.Entities;

public enum StatusProcessamento
{
    EmProcessamento = 1,
    Processado,
    Erro
}

public class ProcessamentoVideo : Entity, IAggregateRoot
{
    public Guid UsuarioId { get; private set; }
    public Arquivo ArquivoDownload { get; private set; }
    public StatusProcessamento Status { get; private set; }
    public DateTime Data { get; private set; }
    public IList<string> Mensagens { get; private set; }

    public ProcessamentoVideo(Guid id,
                              Guid usuarioId,
                              Arquivo arquivoDownload,
                              StatusProcessamento status,
                              DateTime data,
                              IList<string> mensagens)
    {
        Id = id;
        UsuarioId = usuarioId;
        ArquivoDownload = arquivoDownload;
        Status = status;
        Data = data;
        Mensagens = mensagens;
    }

    public ProcessamentoVideo(Guid usuarioId)
    {
        Mensagens = new List<string>();
        ArquivoDownload = new Arquivo();
        UsuarioId = usuarioId;
        AtualizarStatus(StatusProcessamento.EmProcessamento);
        Mensagens.Add("Processamento de vídeos iniciado!");
    }

    public void Finalizar(Arquivo arquivoDownload)
    {
        ArquivoDownload = arquivoDownload;
        AtualizarStatus(StatusProcessamento.Processado);
        Mensagens.Add("Processamento de videos finalizado!");
    }

    public void AdicionarErroProcessamento(string erro)
    {
        AtualizarStatus(StatusProcessamento.Erro);
        Mensagens.Add($"Ocorreu um erro ao processar os vídeos: {erro}.");
    }

    public bool VerificarDownloadDisponivel()
    {
        return ArquivoDownload != null &&
               !string.IsNullOrEmpty(ArquivoDownload.Diretorio) &&
               !string.IsNullOrEmpty(ArquivoDownload.Nome);
    }

    private void AtualizarStatus(StatusProcessamento status)
    {
        Status = status;
        Data = DateTime.UtcNow;
    }
}
