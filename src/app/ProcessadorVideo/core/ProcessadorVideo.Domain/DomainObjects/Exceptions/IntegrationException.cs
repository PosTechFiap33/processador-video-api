namespace ProcessadorVideo.Domain.DomainObjects.Exceptions;

public class IntegrationException : Exception
{
    public IntegrationException(string message) : base(message)
    {
    }
}
