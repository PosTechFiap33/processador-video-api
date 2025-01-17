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

    public void Dispose()
    {
        _context.Dispose();
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

        // Fazendo a consulta
        var response = await _context.Client.GetItemAsync(request);

        if (response.Item == null || !response.Item.Any())
            return null;

        return ProcessamentoVideoDbMappingFactory.MapToEntity(response.Item);
    }
}
