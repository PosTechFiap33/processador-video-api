using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.DomainObjects.Exceptions;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Identity.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    public readonly IdentityContext _context;
    private readonly ILogger<UsuarioRepository> _logger;

    public UsuarioRepository(IdentityContext context, 
                             ILogger<UsuarioRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<Usuario> Consultar(string nomeIdentificacao)
    {
        try
        {
            return await _context.Usuario
                                 .AsNoTracking()
                                 .Where(u => u.NomeIdentificacao == nomeIdentificacao)
                                 .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao consultar os dados do usuario:{ex.Message}");
            throw new IntegrationException($"Ocorreu um erro ao consultar os dados do usuaio!");
        }
    }

    public void Criar(Usuario usuario)
    {
        _context.Usuario.Add(usuario);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
