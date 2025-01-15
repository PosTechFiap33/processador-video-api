using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ProcessadorVideo.Domain.Adapters.MessageBus.Messages;


public class Video
{
    [JsonPropertyName("Diretorio")]
    public string Diretorio { get; set; }

    [JsonPropertyName("Nome")]
    public string Nome { get; set; }

    public Video()
    {
    }

    public Video(string diretorio, string nome)
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
    public IList<Video> Videos { get; set; }

    public ProcessarVideoMessage()
    {
    }

    public ProcessarVideoMessage(Guid processamentoId)
    {
        ProcessamentoId = processamentoId;
        Videos = new List<Video>();
    }

    public void AdicionarVideo(string nomeArquivo, string diretorio)
    {
        var video = new Video(diretorio, nomeArquivo);
        Videos.Add(video);
    }
}
