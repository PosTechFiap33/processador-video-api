using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Domain.Adapters.Repositories;

public interface IProcessamentoPedidoRepository : IRepository<ProcessamentoVideo>
{
    void Criar(ProcessamentoVideo processamentoVideo);
}
