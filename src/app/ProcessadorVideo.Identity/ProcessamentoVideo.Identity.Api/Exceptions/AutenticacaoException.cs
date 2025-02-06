using ProcessadorVideo.Domain.DomainObjects;

namespace ProcessadorVideo.Identity.Api.Exceptions;

public class AutenticacaoException : DomainException
{
    public AutenticacaoException(string message) : base(message)
    {
    }
}
