using System;

namespace ProcessadorVideo.Domain.DomainObjects.Exceptions;

public class ArquivoNaoEncontradoException : DomainException
{
    public ArquivoNaoEncontradoException(string message) : base(message)
    {
    }
}
