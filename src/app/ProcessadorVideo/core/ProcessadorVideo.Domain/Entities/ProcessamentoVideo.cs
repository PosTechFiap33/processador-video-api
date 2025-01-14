using ProcessadorVideo.Domain.DomainObjects;

namespace ProcessadorVideo.Domain.Entities;

public enum StatusProcessamento
{
    EmProcessamento,
    Processado,
    Erro
}

public class ProcessamentoVideo : Entity, IAggregateRoot
{
    public Guid UsuarioId { get; private set; }
    public string UrlDownload { get; private set; }
    public StatusProcessamento Status { get; private set; }
    public DateTime Data { get; private set; }
    public IList<string> Mensagens { get; private set; }

    public ProcessamentoVideo(Guid usuarioId)
    {
        Mensagens = new List<string>();
        UsuarioId = usuarioId;
        AtualizarStatus(StatusProcessamento.EmProcessamento);
        Mensagens.Add("Processamento de vídeos iniciado!");
    }

    public void Finalizar(string urlDownload)
    {
        UrlDownload = urlDownload;
        AtualizarStatus(StatusProcessamento.Processado);
        Mensagens.Add("Processamento de videos finalizado!");
    }

    public void AdicionarErroProcessamento(string erro)
    {
        AtualizarStatus(StatusProcessamento.Erro);
        Mensagens.Add($"Ocorreu um erro ao processar os vídeos: {erro}.");
    }

    private void AtualizarStatus(StatusProcessamento status)
    {
        Status = status;
        Data = DateTime.UtcNow;
    }
}
