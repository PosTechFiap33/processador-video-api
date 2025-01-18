using Amazon.DynamoDBv2.Model;
using ProcessadorVideo.Domain.DomainObjects;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Data.Mappings;

public interface IDynamoEntity<T> where T : Entity, IAggregateRoot
{
    T Entity { get; }

    string GetUpdateExpression();
    Dictionary<string, string> MapAttributesNames();
    Dictionary<string, AttributeValue> MapExpressionAttributesValues();
    public Dictionary<string, AttributeValue> MapToDynamo();
    public T MapToEntity(Dictionary<string, AttributeValue> attributes);
}