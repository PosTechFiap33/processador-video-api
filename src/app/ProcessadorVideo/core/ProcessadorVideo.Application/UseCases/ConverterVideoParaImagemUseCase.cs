using Microsoft.AspNetCore.Http;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.CrossCutting.Extensions;
using ProcessadorVideo.Domain.Adapters.MessageBus;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Domain.Entities;
using Microsoft.Extensions.Options;

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
    private readonly AWSConfiguration _awsConfiguration;

    public ConverterVideoParaImagemUseCase(IMessageBus messageBus,
                                           IFileStorageService fileStorageService,
                                           IProcessamentoVideoRepository repository,
                                           IOptions<AWSConfiguration> configuration)
    {
        _messageBus = messageBus;
        _fileStorageService = fileStorageService;
        _repository = repository;
        _awsConfiguration = configuration.Value;
    }

    public async Task Executar(ICollection<IFormFile> videos, Guid usuarioId)
    {
        var processamento = new ProcessamentoVideo(usuarioId);
        var converterVideoMessage = new ProcessarVideoMessage(processamento.Id);

        string tempDirectory = Path.Combine(Path.GetTempPath(), processamento.Id.ToString());

        try
        {
            var pathProcessamento = $"postech33-processamento-videos/{processamento.Id}";

            await Parallel.ForEachAsync(videos, async (video, CancellationToken) =>
                       {
                           using (var memoryStream = new MemoryStream())
                           {
                               await video.CopyToAsync(memoryStream);
                               await _fileStorageService.Salvar(pathProcessamento, video.FileName, memoryStream.ToArray(), video.ContentType);
                           }

                           converterVideoMessage.AdicionarVideo(video.FileName, pathProcessamento);
                       });

            //TODO: pensar em como deixar atomico
            _repository.Criar(processamento);

            await _repository.UnitOfWork.Commit();

            await _messageBus.PublishAsync(converterVideoMessage, _awsConfiguration.ConverterVideoParaImagemQueueUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        finally
        {
            DirectoryExtensions.RemoveDirectory(tempDirectory);
        }
    }
}
