using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Domain.Adapters.Repositories;

public interface IProcessamentoVideoRepository : IRepository<ProcessamentoVideo>
{
    void Criar(ProcessamentoVideo processamentoVideo);
    void Atualizar(ProcessamentoVideo processamento);
    Task<ProcessamentoVideo?> Consultar(Guid id);
    Task<IEnumerable<ProcessamentoVideo>> ListarPorUsuario(Guid usuarioId);
}
