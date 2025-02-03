using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.Domain.Adapters.Repositories;

namespace ProcessadorVideo.Data;

public class ProcessamentoVideoDynamoContext : IUnitOfWork
{
    public IAmazonDynamoDB Client { get; private set; }
    public readonly List<TransactWriteItem> WriteOperations;

    public ProcessamentoVideoDynamoContext(IOptions<AWSConfiguration> configuration)
    {
        var awsConfig = configuration.Value;

        var credentials = new SessionAWSCredentials(awsConfig.AccesKey, awsConfig.Secret, awsConfig.Token);

        var sqsConfigClient = new AmazonDynamoDBConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(awsConfig.Region)
        };

        if(!string.IsNullOrEmpty(awsConfig.ServiceUrl))
            sqsConfigClient.ServiceURL = awsConfig.ServiceUrl;

        Client = new AmazonDynamoDBClient(credentials, sqsConfigClient);

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
