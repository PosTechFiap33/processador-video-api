using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.Domain.Adapters.Repositories;

namespace ProcessadorVideo.Data;

public class ProcessamentoVideoDynamoContext : IUnitOfWork
{
    public AmazonDynamoDBClient Client { get; private set; }
    public readonly List<TransactWriteItem> WriteOperations;

    public ProcessamentoVideoDynamoContext(IOptions<AWSConfiguration> configuration)
    {
        var awsConfiguration = configuration.Value;

        var config = new AmazonDynamoDBConfig
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsConfiguration.Region),
            LogMetrics = true
        };

        Client = new AmazonDynamoDBClient(config);

        WriteOperations = new List<TransactWriteItem>();
    }

    public async Task Commit()
    {
        try
        {
            if (!WriteOperations.Any())
                return;

            var transactRequest = new TransactWriteItemsRequest
            {
                TransactItems = WriteOperations
            };

            await Client.TransactWriteItemsAsync(transactRequest);

            WriteOperations.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao realizar commit: {ex.Message}");
            throw;
        }
    }

    public List<AttributeValue> MapToList<T>(IList<T> list, Func<T, AttributeValue> GetAttribute)
    {
        if (list != null && list.Any())
            return list.Select(GetAttribute).ToList();

        return new List<AttributeValue>();
    }

    public void Dispose()
    {
        WriteOperations.Clear();
    }
}
