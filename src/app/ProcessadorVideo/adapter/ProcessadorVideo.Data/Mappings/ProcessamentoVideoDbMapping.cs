using Amazon.DynamoDBv2.Model;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Data.Mappings;

public class ProcessamentoVideoDbMapping : IDynamoEntity<ProcessamentoVideo>
{
    public ProcessamentoVideo Entity { get; private set; }

    public ProcessamentoVideoDbMapping(ProcessamentoVideo processamentoVideo)
    {
        Entity = processamentoVideo;
    }

    public Dictionary<string, AttributeValue> MapToDynamo()
    {
        var mapeamentos = new Dictionary<string, AttributeValue>{
            { nameof(Entity.Id), new AttributeValue {  S = Entity.Id.ToString() }},
            { nameof(Entity.Status), new AttributeValue {  S = Entity.Status.ToString() }},
            { nameof(Entity.Data), new AttributeValue {  S = Entity.Data.ToString() }},
            { nameof(Entity.UrlDownload), new AttributeValue {  S = Entity.UrlDownload ?? "" }},
            { nameof(Entity.Mensagens), new AttributeValue { L = MapToList(Entity.Mensagens, m => new AttributeValue { S = m }) }},
        };

        return mapeamentos;
    }

    private List<AttributeValue> MapToList<T>(IList<T> list, Func<T, AttributeValue> GetAttribute)
    {
        if (list != null && list.Any())
            return list.Select(GetAttribute).ToList();

        return new List<AttributeValue>();
    }
}
