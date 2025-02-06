using Microsoft.AspNetCore.Http;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.CrossCutting.Extensions;
using ProcessadorVideo.Domain.Adapters.MessageBus;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace ProcessadorVideo.Application.UseCases;

public interface IConverterVideoParaImagemUseCase
{
    Task<ProcessamentoVideo> Executar(ICollection<IFormFile> videos, Guid usuarioId);
}

public class ConverterVideoParaImagemUseCase : IConverterVideoParaImagemUseCase
{
    private readonly IMessageBus _messageBus;
    private readonly IFileStorageService _fileStorageService;
    private readonly IProcessamentoVideoRepository _repository;
    private readonly AWSConfiguration _awsConfiguration;
    private readonly ILogger<ConverterVideoParaImagemUseCase> _logger;

    public ConverterVideoParaImagemUseCase(IMessageBus messageBus,
                                           IFileStorageService fileStorageService,
                                           IProcessamentoVideoRepository repository,
                                           IOptions<AWSConfiguration> configuration,
                                           ILogger<ConverterVideoParaImagemUseCase> logger)
    {
        _messageBus = messageBus;
        _fileStorageService = fileStorageService;
        _repository = repository;
        _awsConfiguration = configuration.Value;
        _logger = logger;
    }

    public async Task<ProcessamentoVideo> Executar(ICollection<IFormFile> videos, Guid usuarioId)
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
            await _repository.Criar(processamento);

            await _messageBus.PublishAsync(converterVideoMessage, _awsConfiguration.ConverterVideoParaImagemQueueUrl);

            return processamento;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu ao executar o caso de uso: {ex.Message}");
            throw;
        }
        finally
        {
            DirectoryExtensions.RemoveDirectory(tempDirectory);
        }
    }
}
