using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Domain.DomainObjects.Exceptions;

namespace ProcessadorVideo.Infra.Services;

public class BucketS3StorageService : IFileStorageService
{
    private readonly IAmazonS3 _client;

    public BucketS3StorageService(IOptions<AWSConfiguration> configuration,
                                  ILogger<BucketS3StorageService> logger)
    {
        try
        {
            var awsConfig = configuration.Value;

            var credentials = new SessionAWSCredentials(awsConfig.AccesKey, awsConfig.Secret, awsConfig.Token);

            var sqsConfigClient = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(awsConfig.Region),
            };

            if (!string.IsNullOrEmpty(awsConfig.ServiceUrl))
                sqsConfigClient.ServiceURL = awsConfig.ServiceUrl;

            _client = new AmazonS3Client(credentials, sqsConfigClient);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Ocorreu um erro ao criar as credencias da aws: {ex.Message}");
            throw new IntegrationException("Ocorreu um erro ao comunicar com o provedor de cloud!");
        }
    }

    public async Task<byte[]> Ler(string path, string fileName)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = path,
                Key = fileName
            };

            var response = await _client.GetObjectAsync(request);

            byte[] fileBytes;

            using (var memoryStream = new MemoryStream())
            {
                await response.ResponseStream.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            return fileBytes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao ler o arquivo: {ex.Message}");
            throw;
        }
    }

    public async Task Remover(string path, string fileName)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = path,
                Key = fileName
            };

            await _client.DeleteObjectAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao ler o arquivo: {ex.Message}");
            throw;
        }
    }

    public async Task Salvar(string path, string fileName, byte[] fileBytes, string contentType)
    {
        var stream = new MemoryStream(fileBytes);

        try
        {
            var request = new PutObjectRequest
            {
                BucketName = path,
                Key = fileName,
                InputStream = stream,
                ContentType = contentType,
                TagSet = new List<Tag> {
                    new Tag {
                        Key = "ExpirationDate",
                        Value = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }
                }
            };

            await _client.PutObjectAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar arquivo: {ex.Message}");
        }
    }
}
