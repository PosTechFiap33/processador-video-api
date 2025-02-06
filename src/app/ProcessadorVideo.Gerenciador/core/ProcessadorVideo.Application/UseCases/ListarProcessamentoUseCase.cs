using ProcessadorVideo.Application.DTOs;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Application.UseCases;

public interface IListarProcessamentoUseCase
{
    Task<IEnumerable<ProcessamentoVideoDTO>> Executar(Guid usuarioId);
}

public class ListarProcessamentoUseCase : IListarProcessamentoUseCase
{
    private readonly IProcessamentoVideoRepository _repository;

    public ListarProcessamentoUseCase(IProcessamentoVideoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProcessamentoVideoDTO>>  Executar(Guid usuarioId)
    {
        IEnumerable<ProcessamentoVideo> processamentos = await _repository.ListarPorUsuario(usuarioId);

        return processamentos.Select(p => new ProcessamentoVideoDTO(p));
    }
}
