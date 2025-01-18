using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
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
    private readonly DynamoDBContext _context;

    private readonly List<TransactWriteItem> _writeOperations;

    public ProcessamentoVideoDynamoContext(IOptions<AWSConfiguration> configuration)
    {
        var awsConfiguration = configuration.Value;

        var config = new AmazonDynamoDBConfig
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsConfiguration.Region),
        };

        Client = new AmazonDynamoDBClient(config.CreateCredentials(awsConfiguration), config);

        _writeOperations = new List<TransactWriteItem>();
    }

    public async Task Commit()
    {
        try
        {
            if (!_writeOperations.Any())
                return;

            var transactRequest = new TransactWriteItemsRequest
            {
                TransactItems = _writeOperations
            };

            await Client.TransactWriteItemsAsync(transactRequest);

            _writeOperations.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao realizar commit: {ex.Message}");
            throw;
        }
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

    public void Remove<T>(IDynamoEntity<T> dynamoEntity, string tableName) where T : Entity, IAggregateRoot
    {
        var key = new Dictionary<string, AttributeValue>
        {
            { nameof(dynamoEntity.Entity.Id), new AttributeValue { S = dynamoEntity.Entity.Id.ToString() } }
        };

        _writeOperations.Add(new TransactWriteItem
        {
            Delete = new Delete
            {
                TableName = tableName,
                Key = key
            }
        });
    }

    public void Update<T>(IDynamoEntity<T> dynamoEntity, string tableName) where T : Entity, IAggregateRoot
    {
        var key = new Dictionary<string, AttributeValue>
        {
            { nameof(dynamoEntity.Entity.Id), new AttributeValue { S = dynamoEntity.Entity.Id.ToString() } }
        };
        
         var updateItem = new TransactWriteItem
        {
            Update = new Update
            {
                TableName = tableName,
                Key = key,
                UpdateExpression = dynamoEntity.GetUpdateExpression(),
                ExpressionAttributeNames = dynamoEntity.MapAttributesNames(),
                ExpressionAttributeValues = dynamoEntity.MapExpressionAttributesValues()
            }
        };
        _writeOperations.Add(updateItem);
    }

    public void Dispose()
    {
        _writeOperations.Clear();
    }
}
