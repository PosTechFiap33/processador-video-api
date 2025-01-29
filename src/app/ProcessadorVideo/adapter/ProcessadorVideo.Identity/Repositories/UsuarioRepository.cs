using Microsoft.EntityFrameworkCore;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Identity.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    public readonly IdentityContext _context;

    public UsuarioRepository(IdentityContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Usuario> Consultar(string nomeIdentificacao)
    {
        return await _context.Usuario
                             .AsNoTracking()
                             .Where(u => u.NomeIdentificacao == nomeIdentificacao)
                             .FirstOrDefaultAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
