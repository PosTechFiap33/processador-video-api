using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.CrossCutting.Extensions;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Adapters.Services;
using System.IO.Compression;
namespace ProcessadorVideo.Infra.Messaging.Workers;

public class ConverterVideoParaImagemMessagingWorker : MessagingWorker<ProcessarVideoMessage>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IVideoService _videoService;

    public ConverterVideoParaImagemMessagingWorker(ILogger<MessagingWorker<ProcessarVideoMessage>> logger,
                                         IServiceProvider serviceProvider,
                                         IOptions<AWSConfiguration> options,
                                         IFileStorageService fileStorageService,
                                         IVideoService videoService) : base(logger, serviceProvider, $"{options.Value.ProcessarVideoQueueUrl}")
    {
        _fileStorageService = fileStorageService;
        _videoService = videoService;
    }

    protected override async Task ProccessMessage(ProcessarVideoMessage message, IServiceScope serviceScope)
    {
        var frameZipName = $"frames_{Guid.NewGuid()}.zip";
        string zipFilePath = Path.Combine(Path.GetTempPath(), frameZipName);

        string outputFolder = Path.Combine(Path.GetTempPath(), $"{message.ProcessamentoId}/ExtractedFrames");

        try
        {
            await Parallel.ForEachAsync(message.Videos, async (video, CancellationToken) =>
            {
                var videoBytes = await _fileStorageService.Ler(video.Diretorio, video.Nome);
                await _videoService.GenerateImageFromFrames(videoBytes, video.Nome, 20, outputFolder);
                await _fileStorageService.Remover(video.Diretorio, video.Nome);
            });

            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);

            var arquivo = await File.ReadAllBytesAsync(zipFilePath);

            using (MemoryStream memoryStream = new MemoryStream(arquivo))
                await _fileStorageService.Salvar($"postech33-processamento-videos/{message.ProcessamentoId}", frameZipName, memoryStream.ToArray(), "application/zip");

            //TODO - emitir evento de atualizacao do status do processamento na fila
        }
        finally
        {
            DirectoryExtensions.RemoveDirectory(outputFolder);
            DirectoryExtensions.RemoveDirectory(zipFilePath);
        }
    }
}
