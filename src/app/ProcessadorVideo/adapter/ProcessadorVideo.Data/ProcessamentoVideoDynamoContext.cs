using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.CrossCutting.Factories;
using ProcessadorVideo.Data.Mappings;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.DomainObjects;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Data;

public class ProcessamentoVideoDynamoContext : IUnitOfWork
{
    public AmazonDynamoDBClient Client { get; private set; }
    private readonly List<TransactWriteItem> _writeOperations;
    // private readonly ILogger<PagamentoDynamoDbContext> _logger;

    public ProcessamentoVideoDynamoContext(IOptions<AWSConfiguration> configuration)
    {
        // _logger = logger;
        var awsConfiguration = configuration.Value;

        var config = new AmazonDynamoDBConfig
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsConfiguration.Region),
        };

        Client = new AmazonDynamoDBClient(config.CreateCredentials(awsConfiguration), config);

        _writeOperations = new List<TransactWriteItem>();
    }

    public async Task<bool> Commit()
    {
        if (!_writeOperations.Any())
            return true;

        var transactRequest = new TransactWriteItemsRequest
        {
            TransactItems = _writeOperations
        };

        var response = await Client.TransactWriteItemsAsync(transactRequest);

        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            return false;

        return true;
    }

    public void Add<T>(IDynamoEntity<T> dynamoEntity, string tableName) where T : Entity, IAggregateRoot
    {
        _writeOperations.Add(new TransactWriteItem
        {
            Put = new Put
            {
                TableName = tableName,
                Item = dynamoEntity.MapToDynamo()
            }
        });
    }

    public void Dispose()
    {
        _writeOperations.Clear();
    }
}
