using Microsoft.AspNetCore.Http;
using ProcessadorVideo.CrossCutting.Extensions;
using ProcessadorVideo.Domain.Adapters.MessageBus;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Application.UseCases;

public interface IConverterVideoParaImagemUseCase
{
    Task Executar(ICollection<IFormFile> videos, Guid usuarioId);
}

public class ConverterVideoParaImagemUseCase : IConverterVideoParaImagemUseCase
{
    private readonly IMessageBus _messageBus;
    private readonly IFileStorageService _fileStorageService;
    private readonly IProcessamentoVideoRepository _repository;

    public ConverterVideoParaImagemUseCase(IMessageBus messageBus,
                                           IFileStorageService fileStorageService,
                                           IProcessamentoVideoRepository repository)
    {
        _messageBus = messageBus;
        _fileStorageService = fileStorageService;
        _repository = repository;
    }

    public async Task Executar(ICollection<IFormFile> videos, Guid usuarioId)
    {
        // string outputFolder = Path.Combine(Path.GetTempPath(), "ExtractedFrames");
        // Directory.CreateDirectory(outputFolder);

        // var frameZipName = $"frames_{Guid.NewGuid()}.zip";
        // string zipFilePath = Path.Combine(Path.GetTempPath(), frameZipName);

        var processamento = new ProcessamentoVideo(usuarioId);
        var converterVideoMessage = new ProcessarVideoMessage(processamento.Id);

        string tempDirectory = Path.Combine(Path.GetTempPath(), processamento.Id.ToString());

        try
        {
            var pathProcessamento = $"postech33-processamento-videos/{processamento.Id}";

            await Parallel.ForEachAsync(videos, async (video, CancellationToken) =>
                       {
                           //    using (var stream = new FileStream(tempDirectory, FileMode.Create))
                           using (var memoryStream = new MemoryStream())
                           {
                               await video.CopyToAsync(memoryStream);
                               await _fileStorageService.Salvar(pathProcessamento, video.FileName, memoryStream.ToArray(), video.ContentType);
                           }

                           converterVideoMessage.AdicionarVideo(video.FileName, pathProcessamento);
                       });

            _repository.Criar(processamento);

            await _repository.UnitOfWork.Commit();

            await _messageBus.PublishAsync(converterVideoMessage, "converter-video-para-imagem");

            // await Parallel.ForEachAsync(videos, async (video, CancellationToken) =>
            //            {
            //                await _videoService.GenerateImageFromFrames(video, 20, outputFolder);
            //            });

            // ZipFile.CreateFromDirectory(outputFolder, zipFilePath);

            //  return await File.ReadAllBytesAsync(zipFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        finally
        {
            DirectoryExtensions.RemoveDirectory(tempDirectory);
            // DirectoryExtensions.RemoveDirectory(outputFolder);
            // DirectoryExtensions.RemoveDirectory(zipFilePath);
        }
    }
}
