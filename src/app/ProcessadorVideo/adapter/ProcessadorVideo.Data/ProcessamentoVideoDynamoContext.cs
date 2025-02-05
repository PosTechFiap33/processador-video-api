using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.Domain.DomainObjects.Exceptions;

namespace ProcessadorVideo.Data;

public class ProcessamentoVideoDynamoContext 
{
    public IAmazonDynamoDB Client { get; private set; }
    // public readonly List<TransactWriteItem> WriteOperations;
    private readonly ILogger<ProcessamentoVideoDynamoContext> _logger;

    public ProcessamentoVideoDynamoContext(IOptions<AWSConfiguration> configuration,
                                           ILogger<ProcessamentoVideoDynamoContext> logger)
    {
        try
        {
            _logger = logger;

            var awsConfig = configuration.Value;

            var credentials = new SessionAWSCredentials(awsConfig.AccesKey, awsConfig.Secret, awsConfig.Token);

            var sqsConfigClient = new AmazonDynamoDBConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsConfig.Region)
            };

            if (!string.IsNullOrEmpty(awsConfig.ServiceUrl))
                sqsConfigClient.ServiceURL = awsConfig.ServiceUrl;

            Client = new AmazonDynamoDBClient(credentials, sqsConfigClient);

            // WriteOperations = new List<TransactWriteItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao criar as credencias da aws: {ex.Message}");
            throw new IntegrationException("Ocorreu um erro ao comunicar com o provedor de cloud!");
        }
    }

    // public async Task Commit()
    // {
    //     try
    //     {
    //         if (!WriteOperations.Any())
    //             return;

    //         var transactRequest = new TransactWriteItemsRequest
    //         {
    //             TransactItems = WriteOperations
    //         };

    //         await Client.TransactWriteItemsAsync(transactRequest);

    //         WriteOperations.Clear();
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, $"Erro ao realizar commit: {ex.Message}");
    //         throw new IntegrationException("Ocorreu um erro interno, por favor tente novamente!");
    //     }
    // }

    public List<AttributeValue> MapToList<T>(IList<T> list, Func<T, AttributeValue> GetAttribute)
    {
        if (list != null && list.Any())
            return list.Select(GetAttribute).ToList();

        return new List<AttributeValue>();
    }

    // public void Dispose()
    // {
    //     WriteOperations.Clear();
    // }
}
