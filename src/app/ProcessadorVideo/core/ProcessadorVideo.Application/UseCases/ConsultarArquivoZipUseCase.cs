using ProcessadorVideo.Application.DTOs;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Domain.DomainObjects;
using ProcessadorVideo.Domain.DomainObjects.Exceptions;

namespace ProcessadorVideo.Application.UseCases;

public interface IConsultarArquivoZipUseCase
{
    Task<ArquivoZipDTO> Executar(Guid processamentoId);
}

public class ConsultarArquivoZipUseCase : IConsultarArquivoZipUseCase
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IProcessamentoVideoRepository _repository;

    public ConsultarArquivoZipUseCase(IFileStorageService fileStorageService,
                                      IProcessamentoVideoRepository repository)
    {
        _fileStorageService = fileStorageService;
        _repository = repository;
    }

    public async Task<ArquivoZipDTO> Executar(Guid processamentoId)
    {
        var processamento = await _repository.Consultar(processamentoId);

        if(processamento is null)
            throw new ProcessamentoNaoEncontradoException($"Não foi encontrado um processamento com id {processamentoId}");

        if (processamento.ArquivoDownload is null)
            throw new ArquivoNaoEncontradoException($"Não foi encontrado um arquivo para o processamento {processamentoId}!");

        var arquivoDownload = processamento.ArquivoDownload;

        var zipBytes = await _fileStorageService.Ler(processamento.ArquivoDownload.Diretorio, processamento.ArquivoDownload.Nome);

        return new ArquivoZipDTO
        {
            Nome = arquivoDownload.Nome,
            Conteudo = zipBytes
        };
    }
}
