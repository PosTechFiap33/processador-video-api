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

            // Criando o request para obter o objeto
            var request = new GetObjectRequest
            {
                BucketName = path,
                Key = fileName
            };

            // Obter o objeto
            using (var response = await client.GetObjectAsync(request))
            {
                // Lendo o conteúdo do arquivo em um MemoryStream
                using (var memoryStream = new MemoryStream())
                {
                    await response.ResponseStream.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);  // Voltar para o início do stream

                    // Agora você pode usar o conteúdo do arquivo
                    return memoryStream.ToArray();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao ler o arquivo: {ex.Message}");
            throw;
        }
    }

    public async Task Salvar(string path, string fileName, Stream stream)
    {
        var client = CriarClient();

        await CriarBucket(client, path);

        try
        {
            var request = new PutObjectRequest
            {
                BucketName = path,
                Key = fileName,
                InputStream = stream
            };

            await client.PutObjectAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar arquivo: {ex.Message}");
        }
    }

    private async Task CriarBucket(IAmazonS3 s3Client, string bucketName)
    {
        try
        {
            var request = new PutBucketRequest
            {
                BucketName = bucketName
            };

            await s3Client.PutBucketAsync(request);
            Console.WriteLine($"Bucket {bucketName} criado com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar o bucket: {ex.Message}");
            throw ex;
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
