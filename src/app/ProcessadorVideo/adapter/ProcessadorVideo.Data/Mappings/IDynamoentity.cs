using Amazon.DynamoDBv2.Model;
using ProcessadorVideo.Domain.DomainObjects;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Data.Mappings;

public interface IDynamoEntity<T> where T : Entity, IAggregateRoot
{
    T Entity { get; }
    public Dictionary<string, AttributeValue> MapToDynamo();
}