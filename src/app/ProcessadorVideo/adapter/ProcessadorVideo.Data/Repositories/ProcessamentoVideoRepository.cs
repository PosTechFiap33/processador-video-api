using Amazon.DynamoDBv2.Model;
using ProcessadorVideo.Data.Mappings;
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
        _context.Add(new ProcessamentoVideoDbMapping(processamentoVideo), TABLE_NAME);
    }

    public void Atualizar(ProcessamentoVideo processamento)
    {
        _context.Update(new ProcessamentoVideoDbMapping(processamento), TABLE_NAME);
    }


    public async Task<ProcessamentoVideo?> Consultar(Guid id)
    {
        var request = new GetItemRequest
        {
            TableName = TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>
            {
                { nameof(ProcessamentoVideo.Id), new AttributeValue { S = id.ToString() } }
            }
        };

        var response = await _context.Client.GetItemAsync(request);

        if (response.Item == null || !response.Item.Any())
            return null;

        return ProcessamentoVideoDbMappingFactory.MapToEntity(response.Item);
    }

    public async Task<IEnumerable<ProcessamentoVideo>> ListarPorUsuario(Guid usuarioId)
    {
        var request = new QueryRequest
        {
            TableName = TABLE_NAME,
            KeyConditionExpression = "UsuarioId = :usuarioId",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":usuarioId", new AttributeValue { S = usuarioId.ToString() } }
            }
        };

        var response = await _context.Client.QueryAsync(request);

        if (response.Items == null || !response.Items.Any())
            return Enumerable.Empty<ProcessamentoVideo>();

        return response.Items.Select(ProcessamentoVideoDbMappingFactory.MapToEntity);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
