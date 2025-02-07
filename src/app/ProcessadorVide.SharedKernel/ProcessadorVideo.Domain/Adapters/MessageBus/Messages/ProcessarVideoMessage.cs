using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ProcessadorVideo.Domain.Adapters.MessageBus.Messages;


public class Arquivo
{
    [JsonPropertyName("Diretorio")]
    public string Diretorio { get; set; }

    [JsonPropertyName("Nome")]
    public string Nome { get; set; }

    public Arquivo()
    {
    }

    public Arquivo(string diretorio, string nome)
    {
        Diretorio = diretorio;
        Nome = nome;
    }
}

[ExcludeFromCodeCoverage]
public class ProcessarVideoMessage
{
    [JsonPropertyName("ProcessamentoId")]
    public Guid ProcessamentoId { get; set; }

    [JsonPropertyName("Videos")]
    public IList<Arquivo> Videos { get; set; }

    public ProcessarVideoMessage()
    {
    }

    public ProcessarVideoMessage(Guid processamentoId)
    {
        ProcessamentoId = processamentoId;
        Videos = new List<Arquivo>();
    }

    public void AdicionarVideo(string nomeArquivo, string diretorio)
    {
        var video = new Arquivo(diretorio, nomeArquivo);
        Videos.Add(video);
    }
}
