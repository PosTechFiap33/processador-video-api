using System;
using System.Text.Json.Serialization;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Application.DTOs;

public class ProcessamentoVideoDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; private set; }

    [JsonPropertyName("status")]
    public StatusProcessamento Status { get; private set; }

    [JsonPropertyName("Mensagens")]
    public ICollection<string> Mensagens { get; private set; }

    public ProcessamentoVideoDTO(ProcessamentoVideo processamento)
    {
        Id = processamento.Id;
        Status = processamento.Status;
        Mensagens = processamento.Mensagens;
    }
}
