using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Domain.Adapters.Repositories;

public interface IProcessamentoVideoRepository 
{
    Task Criar(ProcessamentoVideo processamentoVideo);
    Task Atualizar(ProcessamentoVideo processamento);
    Task<ProcessamentoVideo?> Consultar(Guid id);
    Task<IEnumerable<ProcessamentoVideo>> ListarPorUsuario(Guid usuarioId);
}
