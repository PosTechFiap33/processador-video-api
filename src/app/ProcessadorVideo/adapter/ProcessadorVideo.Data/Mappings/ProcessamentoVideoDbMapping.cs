using System.Text;
using Amazon.DynamoDBv2.Model;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Data.Mappings;

public class ProcessamentoVideoDbMapping : IDynamoEntity<ProcessamentoVideo>
{
    public ProcessamentoVideo Entity { get; private set; }

    public ProcessamentoVideoDbMapping(ProcessamentoVideo processamentoVideo)
    {
        Entity = processamentoVideo;
    }

    internal ProcessamentoVideoDbMapping()
    {
    }

    public Dictionary<string, AttributeValue> MapToDynamo()
    {
        var arquivoMapeamento = new Dictionary<string, AttributeValue>{
            {nameof(Entity.ArquivoDownload.Diretorio), new AttributeValue {  S = Entity.ArquivoDownload.Diretorio ?? "" }},
            {nameof(Entity.ArquivoDownload.Nome), new AttributeValue {  S = Entity.ArquivoDownload.Nome ?? "" }}
        };

        var mapeamentos = new Dictionary<string, AttributeValue>{
            { nameof(Entity.Id), new AttributeValue {  S = Entity.Id.ToString() }},
            { nameof(Entity.UsuarioId), new AttributeValue {  S = Entity.UsuarioId.ToString() }},
            { nameof(Entity.Status), new AttributeValue {  N = ((int)Entity.Status).ToString() }},
            { nameof(Entity.Data), new AttributeValue {  S = Entity.Data.ToString() }},
            { nameof(Entity.ArquivoDownload), new AttributeValue {  M = arquivoMapeamento}},
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

    public ProcessamentoVideo MapToEntity(Dictionary<string, AttributeValue> attributes)
    {
        var mensagens = attributes[nameof(ProcessamentoVideo.Mensagens)].L;

        var arquivoAttributes = attributes[nameof(ProcessamentoVideo.ArquivoDownload)].M;
        Arquivo arquivo = new Arquivo();

        arquivo = new Arquivo
        {
            Nome = arquivoAttributes[nameof(Arquivo.Nome)].S,
            Diretorio = arquivoAttributes[nameof(Arquivo.Diretorio)].S
        };

        return new ProcessamentoVideo(
            Guid.Parse(attributes[nameof(ProcessamentoVideo.Id)].S),
            Guid.Parse(attributes[nameof(ProcessamentoVideo.UsuarioId)].S),
            arquivo,
            (StatusProcessamento)int.Parse(attributes[nameof(ProcessamentoVideo.Status)].N),
            DateTime.Parse(attributes[nameof(ProcessamentoVideo.Data)].S),
            mensagens?.Select(m => m.S)?.ToList()
        );
    }

    public string GetUpdateExpression()
    {
        var updateExpression = new StringBuilder("SET ");
        updateExpression.Append("#status = :status, ");
        updateExpression.Append("#data = :data, ");
        updateExpression.Append("#mensagens = :mensagens, ");
        updateExpression.Append("#arquivoDownload = :arquivoDownload");
        return updateExpression.ToString();
    }

    public Dictionary<string, string> MapAttributesNames()
    {
        return new Dictionary<string, string>
        {
            { "#status", "Status" },
            { "#data", "Data" },
            { "#mensagens", "Mensagens" },
            { "#arquivoDownload", "ArquivoDownload" }
        };        
    }

    public Dictionary<string, AttributeValue> MapExpressionAttributesValues()
    {
        var arquivoMapeamento = new Dictionary<string, AttributeValue>
        {
            { "Diretorio", new AttributeValue { S = Entity.ArquivoDownload.Diretorio ?? "" } },
            { "Nome", new AttributeValue { S = Entity.ArquivoDownload.Nome ?? "" } }
        };

        return new Dictionary<string, AttributeValue>
        {
            { ":status", new AttributeValue { N = ((int)Entity.Status).ToString() } },
            { ":data", new AttributeValue { S = Entity.Data.ToString() } },
            { ":mensagens", new AttributeValue { L = MapToList(Entity.Mensagens, m => new AttributeValue { S = m }) } },
            { ":arquivoDownload", new AttributeValue { M = arquivoMapeamento } }
        };
    }
}

public static class ProcessamentoVideoDbMappingFactory
{
    public static ProcessamentoVideo MapToEntity(Dictionary<string, AttributeValue> attributes)
    {
        return new ProcessamentoVideoDbMapping().MapToEntity(attributes);
    }
}
