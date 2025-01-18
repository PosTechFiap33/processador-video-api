using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.CrossCutting.Factories;
using ProcessadorVideo.Domain.Adapters.Services;

namespace ProcessadorVideo.Infra.Services;

public class BucketS3StorageService : IFileStorageService
{
    private readonly AWSConfiguration _configuration;

    public BucketS3StorageService(IOptions<AWSConfiguration> options)
    {
        _configuration = options.Value;
    }

    public async Task<byte[]> Ler(string path, string fileName)
    {
        try
        {
            var client = CriarClient();

            var request = new GetObjectRequest
            {
                BucketName = path,
                Key = fileName
            };

            var response = await client.GetObjectAsync(request);

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
            var client = CriarClient();

            var request = new DeleteObjectRequest
            {
                BucketName = path,
                Key = fileName
            };

            await client.DeleteObjectAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao ler o arquivo: {ex.Message}");
            throw;
        }
    }

    public async Task Salvar(string path, string fileName, byte[] fileBytes, string contentType)
    {
        var client = CriarClient();

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

            await client.PutObjectAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar arquivo: {ex.Message}");
        }
    }

    private AmazonS3Client CriarClient()
    {
        var config = new AmazonS3Config
        {
            ForcePathStyle = true
        };

        if (!string.IsNullOrEmpty(_configuration.ServiceUrl))
            config.ServiceURL = _configuration.ServiceUrl;

        return new AmazonS3Client(config.CreateCredentials(_configuration), config);
    }
}
