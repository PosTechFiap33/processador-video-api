using System;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Domain.Adapters.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario> Consultar(string nomeIdentificacao);
    void Criar(Usuario usuario);
}
