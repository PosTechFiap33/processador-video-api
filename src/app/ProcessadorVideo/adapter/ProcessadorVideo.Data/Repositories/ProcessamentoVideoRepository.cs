using System.Text;
using Amazon.DynamoDBv2.Model;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Data.Repositories;

public class ProcessamentoVideoRepository : IProcessamentoVideoRepository
{
    private const string TABLE_NAME = "ProcessamentoVideos";
    private readonly ProcessamentoVideoDynamoContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public ProcessamentoVideoRepository(ProcessamentoVideoDynamoContext context)
    {
        _context = context;
    }

    public void Criar(ProcessamentoVideo processamentoVideo)
    {
        var mapeamentos = new Dictionary<string, AttributeValue>{
            { nameof(processamentoVideo.Id), new AttributeValue {  S = processamentoVideo.Id.ToString() }},
            { nameof(processamentoVideo.UsuarioId), new AttributeValue {  S = processamentoVideo.UsuarioId.ToString() }},
            { nameof(processamentoVideo.Status), new AttributeValue {  N = ((int)processamentoVideo.Status).ToString() }},
            { nameof(processamentoVideo.Data), new AttributeValue {  S = processamentoVideo.Data.ToString() }},
            { nameof(processamentoVideo.Mensagens), new AttributeValue { L = _context.MapToList(processamentoVideo.Mensagens, m => new AttributeValue { S = m }) }},
            { nameof(processamentoVideo.ArquivoDownload), new AttributeValue {
                M = new Dictionary<string, AttributeValue>{
                        {nameof(processamentoVideo.ArquivoDownload.Diretorio), new AttributeValue {  S = processamentoVideo.ArquivoDownload.Diretorio ?? "" }},
                        {nameof(processamentoVideo.ArquivoDownload.Nome), new AttributeValue {  S = processamentoVideo.ArquivoDownload.Nome ?? "" }}
                }
            }},
        };

        _context.WriteOperations.Add(new TransactWriteItem
        {
            Put = new Put
            {
                TableName = TABLE_NAME,
                Item = mapeamentos
            }
        });
    }

    public void Atualizar(ProcessamentoVideo processamento)
    {
        var key = new Dictionary<string, AttributeValue>
        {
            { nameof(processamento.Id), new AttributeValue { S = processamento.Id.ToString() } }
        };

        var updateExpression = new StringBuilder("SET ");
        updateExpression.Append("#status = :status, ");
        updateExpression.Append("#data = :data, ");
        updateExpression.Append("#mensagens = :mensagens, ");
        updateExpression.Append("#arquivoDownload = :arquivoDownload");

        var mapAttributesNames = new Dictionary<string, string>
        {
            { "#status", "Status" },
            { "#data", "Data" },
            { "#mensagens", "Mensagens" },
            { "#arquivoDownload", "ArquivoDownload" }
        };


        var mapExpressionAttributesValues = new Dictionary<string, AttributeValue>
        {
            { ":status", new AttributeValue { N = ((int)processamento.Status).ToString() } },
            { ":data", new AttributeValue { S = processamento.Data.ToString() } },
            { ":mensagens", new AttributeValue { L = _context.MapToList(processamento.Mensagens, m => new AttributeValue { S = m }) } },
            { ":arquivoDownload", new AttributeValue {
                    M = new Dictionary<string, AttributeValue> {
                            { "Diretorio", new AttributeValue { S = processamento.ArquivoDownload.Diretorio ?? "" } },
                            { "Nome", new AttributeValue { S = processamento.ArquivoDownload.Nome ?? "" } }
                        }
                }
            }
        };

        var updateItem = new TransactWriteItem
        {
            Update = new Update
            {
                TableName = TABLE_NAME,
                Key = key,
                UpdateExpression = updateExpression.ToString(),
                ExpressionAttributeNames = mapAttributesNames,
                ExpressionAttributeValues = mapExpressionAttributesValues
            }
        };

        _context.WriteOperations.Add(updateItem);
    }

    public async Task<ProcessamentoVideo?> Consultar(Guid id)
    {
        var request = new QueryRequest
        {
            TableName = TABLE_NAME,
            KeyConditionExpression = "Id = :id",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":id", new AttributeValue { S = id.ToString() } }
            }
        };

        var response = await _context.Client.QueryAsync(request);

        if (response.Items == null || !response.Items.Any())
            return null;

        return MapToEntity(response.Items.FirstOrDefault());
    }

    public async Task<IEnumerable<ProcessamentoVideo>> ListarPorUsuario(Guid usuarioId)
    {
        var request = new ScanRequest
        {
            TableName = TABLE_NAME,
            FilterExpression = "UsuarioId = :usuarioId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":usuarioId", new AttributeValue { S = usuarioId.ToString() } }
            }
        };

        var response = await _context.Client.ScanAsync(request);

        if (response.Items == null || !response.Items.Any())
            return Enumerable.Empty<ProcessamentoVideo>();

        return response.Items.Select(MapToEntity);
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

    public void Dispose()
    {
        _context.Dispose();
    }
}
