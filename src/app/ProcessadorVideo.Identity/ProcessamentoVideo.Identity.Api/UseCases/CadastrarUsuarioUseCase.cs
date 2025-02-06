using ProcessadorVideo.CrossCutting.Extensions;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.DomainObjects;
using ProcessadorVideo.Domain.Entities;
using ProcessadorVideo.Identity.Api.DTOs;

namespace ProcessadorVideo.Identity.Api.UseCases;

public interface ICadastrarUsuarioUseCase
{
    Task<Usuario> Executar(UsuarioDTO usuario);
}

public class CadastrarUsuarioUseCase : ICadastrarUsuarioUseCase
{
    private readonly IUsuarioRepository _repository;

    public CadastrarUsuarioUseCase(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<Usuario> Executar(UsuarioDTO dadosUsuario)
    {

        var usuarioCadastrado = await _repository.Consultar(dadosUsuario.Usuario);

        if (usuarioCadastrado is not null)
            throw new DomainException("Ja existe um usuario com esse nome cadastrado!");

        var usuario = new Usuario(dadosUsuario.Usuario,
                                  dadosUsuario.Senha.ToMD5(),
                                  dadosUsuario.Email,
                                  dadosUsuario.Perfil);

        _repository.Criar(usuario);

        await _repository.UnitOfWork.Commit();

        return usuario;
    }
}
