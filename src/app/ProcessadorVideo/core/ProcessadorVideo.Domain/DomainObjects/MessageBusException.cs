namespace ProcessadorVideo.Domain.DomainObjects;

public class MessageBusException : Exception
{
    public MessageBusException(string message) : base(message) { }
}
