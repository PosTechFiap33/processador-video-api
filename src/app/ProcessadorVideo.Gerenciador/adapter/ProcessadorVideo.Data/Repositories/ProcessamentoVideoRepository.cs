using System.Text;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.DomainObjects.Exceptions;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Data.Repositories;

public class ProcessamentoVideoRepository : IProcessamentoVideoRepository
{
    private const string TABLE_NAME = "ProcessamentoVideos";
    private readonly ProcessamentoVideoDynamoContext _context;
    private readonly ILogger<ProcessamentoVideoRepository> _logger;

    public ProcessamentoVideoRepository(ProcessamentoVideoDynamoContext context,
                                        ILogger<ProcessamentoVideoRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Criar(ProcessamentoVideo processamentoVideo)
    {
        try
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

            await _context.Client.PutItemAsync(TABLE_NAME, mapeamentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao cadastrar o item no dynamo: {ex.Message}");
            throw new IntegrationException($"Ocorreu um erro ao cadastrar o item no dynamo: {ex.Message}");
        }
    }

    public async Task Atualizar(ProcessamentoVideo processamento)
    {
        try
        {
            var key = new Dictionary<string, AttributeValue>
            {
                { nameof(processamento.Id), new AttributeValue { S = processamento.Id.ToString() } },
                { nameof(processamento.UsuarioId), new AttributeValue { S = processamento.UsuarioId.ToString() } } 
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

            var updateRequest = new UpdateItemRequest
            {
                TableName = TABLE_NAME,
                Key = key,
                UpdateExpression = updateExpression.ToString(),
                ExpressionAttributeNames = mapAttributesNames,
                ExpressionAttributeValues = mapExpressionAttributesValues
            };

            await _context.Client.UpdateItemAsync(updateRequest);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao atualizar o item no dynamo: {ex.Message}");
            throw new IntegrationException($"Ocorreu um erro ao atualizar o item no dynamo: {ex.Message}");
        }
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

        var arquivo = new Arquivo
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

    // public void Dispose()
    // {
    //     _context.Dispose();
    // }
}
