using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using ProcessadorVideo.CrossCutting.Extensions;
using ProcessadorVideo.Domain.Services;

namespace ProcessadorVideo.Application.UseCases;

public interface IConverterVideoParaImagemUseCase
{
    Task<byte[]> Executar(ICollection<IFormFile> videos, Guid usuarioId);
}

public class ConverterVideoParaImagemUseCase : IConverterVideoParaImagemUseCase
{
    private readonly IVideoService _videoService;

    public ConverterVideoParaImagemUseCase(IVideoService videoService)
    {
        _videoService = videoService;
    }

    public async Task<byte[]> Executar(ICollection<IFormFile> videos, Guid usuarioId)
    {
        string outputFolder = Path.Combine(Path.GetTempPath(), "ExtractedFrames");
        Directory.CreateDirectory(outputFolder);

        var frameZipName = $"frames_{Guid.NewGuid()}.zip";
        string zipFilePath = Path.Combine(Path.GetTempPath(), frameZipName);

        try
        {
            await Parallel.ForEachAsync(videos, async (video, CancellationToken) =>
            {
                await _videoService.GenerateImageFromFrames(video, 20, outputFolder);
            });

            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);

            return await File.ReadAllBytesAsync(zipFilePath);
        }
        finally
        {
            DirectoryExtensions.RemoveDirectory(outputFolder);
            DirectoryExtensions.RemoveDirectory(zipFilePath);
        }
    }
}
