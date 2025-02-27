using System.IO.Compression;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.CrossCutting.Extensions;
using ProcessadorVideo.Domain.Adapters.MessageBus;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Infra.Messaging.Workers;

namespace ProcessadorVideo.Conversor.Worker;

public class ProcessarVideoMessagingWorker : MessagingWorker<ProcessarVideoMessage>
{
    private readonly AWSConfiguration _awsConfiguration;

    public ProcessarVideoMessagingWorker(ILogger<ProcessarVideoMessagingWorker> logger,
                                         IServiceProvider serviceProvider,
                                         IOptions<AWSConfiguration> options)
                                         : base(logger, serviceProvider, $"{options.Value.ConverterVideoParaImagemQueueUrl}", 120)
    {
        _awsConfiguration = options.Value;
    }

    protected override async Task ProccessMessage(ProcessarVideoMessage processarVideoMessage, IServiceScope scope)
    {
        var frameZipName = $"frames_{Guid.NewGuid()}.zip";
        var zipFilePath = Path.Combine(Path.GetTempPath(), frameZipName);
        var outputFolder = Path.Combine(Path.GetTempPath(), $"{processarVideoMessage.ProcessamentoId}/ExtractedFrames");

        var _messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        try
        {
            var _videoService = scope.ServiceProvider.GetRequiredService<IVideoService>();
            var _fileStorageService = scope.ServiceProvider.GetRequiredService<IFileStorageService>();

            await Parallel.ForEachAsync(processarVideoMessage.Videos, async (video, CancellationToken) =>
            {
                var videoBytes = await _fileStorageService.Ler(video.Diretorio, video.Nome);
                await _videoService.GenerateImageFromFrames(videoBytes, video.Nome, 20, outputFolder);
                await _fileStorageService.Remover(video.Diretorio, video.Nome);
            });

            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);

            var arquivo = await File.ReadAllBytesAsync(zipFilePath);

            var diretorioZip = $"postech33-processamento-videos/{processarVideoMessage.ProcessamentoId}";

            using (MemoryStream memoryStream = new MemoryStream(arquivo))
                await _fileStorageService.Salvar(diretorioZip, frameZipName, memoryStream.ToArray(), "application/zip");

            var processamentoVideoRealizadoMessage = new ProcessamentoVideoRealizadoMessage(processarVideoMessage.ProcessamentoId, new Arquivo(diretorioZip, frameZipName));
            await _messageBus.PublishAsync(processamentoVideoRealizadoMessage, _awsConfiguration.ConversaoVideoParaImagemRealizadaQueueUrl);
        }
        catch (Exception ex)
        {
            var erroProcessamentoVideoMessage = new ErroProcessamentoVideoMessage(processarVideoMessage.ProcessamentoId, ex.Message);
            await _messageBus.PublishAsync(erroProcessamentoVideoMessage, _awsConfiguration.ConversaoVideoParaImagemErroQueueUrl);
            _logger.LogError(ex, $"Ocorreu um erro ao processar o video! Id do procesamento: {processarVideoMessage.ProcessamentoId}");
        }
        finally
        {
            DirectoryExtensions.RemoveDirectory(outputFolder);
            DirectoryExtensions.RemoveDirectory(zipFilePath);
        }
    }
}
