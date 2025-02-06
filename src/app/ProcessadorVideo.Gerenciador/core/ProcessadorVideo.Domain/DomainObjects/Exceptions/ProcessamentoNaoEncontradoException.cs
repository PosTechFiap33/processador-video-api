using System;

namespace ProcessadorVideo.Domain.DomainObjects.Exceptions;

public class ProcessamentoNaoEncontradoException : DomainException
{
    public ProcessamentoNaoEncontradoException(string message) : base(message)
    {
    }
}
