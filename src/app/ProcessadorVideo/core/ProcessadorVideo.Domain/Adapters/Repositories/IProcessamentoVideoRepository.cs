using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Domain.Adapters.Repositories;

public interface IProcessamentoVideoRepository : IRepository<ProcessamentoVideo>
{
    void Criar(ProcessamentoVideo processamentoVideo);
}
