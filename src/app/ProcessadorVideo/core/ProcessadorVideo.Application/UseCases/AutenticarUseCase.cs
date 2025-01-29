using Microsoft.Extensions.Logging;
using ProcessadorVideo.CrossCutting.Extensions;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Domain.DomainObjects.Exceptions;

namespace ProcessadorVideo.Application.UseCases;

public interface IAutenticarUseCase
{
    Task<string> Executar(string nomeIdentificacao, string senha);
}

public class AutenticarUseCase : IAutenticarUseCase
{
    private readonly ILogger<AutenticarUseCase> _logger;
    private readonly IUsuarioRepository _repository;
    private readonly ITokenService _tokenService;

    public AutenticarUseCase(IUsuarioRepository repository,
                             ILogger<AutenticarUseCase> logger,
                             ITokenService tokenService)
    {
        _repository = repository;
        _logger = logger;
        _tokenService = tokenService;
    }

    public async Task<string> Executar(string nomeIdentificacao, string senha)
    {
        var usuario = await _repository.Consultar(nomeIdentificacao);

        if (usuario is null)
            ProcessarErro($"usuario {nomeIdentificacao} não encontrado!");

        var autenticacaoRealizada = usuario.Autenticar(senha.ToMD5());

        if (autenticacaoRealizada is false)
            ProcessarErro("Autenticacao inválida para o usuario {nomeIdentificacao}!");

        return _tokenService.Gerar(usuario);
    }

    private void ProcessarErro(string log)
    {
        _logger.LogWarning(log);
        throw new AutenticacaoException("Ocorreu um erro ao realizar a autenticação, verifique o usuário e senha informados!");
    }
}
