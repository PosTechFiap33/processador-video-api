using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;
using ProcessadorVideo.Domain.Adapters.Repositories;

namespace ProcessadorVideo.Data;

public class ProcessamentoVideoDynamoContext : IUnitOfWork
{
    public IAmazonDynamoDB Client { get; private set; }
    public readonly List<TransactWriteItem> WriteOperations;

    public ProcessamentoVideoDynamoContext(IConfiguration configuration)
    {
        var awsConfig = configuration.GetSection("AWS");

        string serviceUrl = awsConfig["ServiceUrl"] ?? string.Empty;
        string region = awsConfig["Region"] ?? "us-east-1";

        var sqsConfigClient = new AmazonDynamoDBConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(region),
            ServiceURL = serviceUrl
        };

        Client = new AmazonDynamoDBClient(sqsConfigClient);

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
