using ProcessadorVideo.Data.Mappings;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Data.Repositories;

public class ProcessamentoPedidoRepository : IProcessamentoPedidoRepository
{
    private const string TABLE_NAME = "ProcessamentoVideos";

    private readonly ProcessamentoVideoDynamoContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public ProcessamentoPedidoRepository(ProcessamentoVideoDynamoContext context)
    {
        _context = context;
    }

    public void Criar(ProcessamentoVideo processamentoVideo)
    {
        _context.Add(new ProcessamentoVideoDbMapping(processamentoVideo), TABLE_NAME);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

}
