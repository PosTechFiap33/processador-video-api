using System;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Application.DTOs;

public class ProcessamentoVideoDTO
{
    public Guid Id { get; private set; }
    public StatusProcessamento Status { get; private set; }
    public ICollection<string> Mensagens { get; private set; }

    public ProcessamentoVideoDTO(ProcessamentoVideo processamento)
    {
        Id = processamento.Id;
        Status = processamento.Status;
        Mensagens = processamento.Mensagens;
    }
}
