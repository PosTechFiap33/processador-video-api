using ProcessadorVideo.CrossCutting.Extensions;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Identity.Api.Exceptions;

namespace ProcessadorVideo.Identity.Api.UseCases;

public interface IAutenticarUseCase
{
    Task<string> Executar(string nomeIdentificacao, string senha);
}

public class AutenticarUseCase : IAutenticarUseCase
{
    private readonly ILogger<AutenticarUseCase> _logger;
    private readonly IUsuarioRepository _repository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AutenticarUseCase(IUsuarioRepository repository,
                             ILogger<AutenticarUseCase> logger,
                             ITokenService tokenService,
                             IConfiguration configuration)
    {
        _repository = repository;
        _logger = logger;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<string> Executar(string nomeIdentificacao, string senha)
    {
        if (string.IsNullOrEmpty(nomeIdentificacao))
            ProcessarErro("Nome de identificação não informado!");

        if (string.IsNullOrEmpty(senha))
            ProcessarErro("Senha não informada!");

        var usuario = await _repository.Consultar(nomeIdentificacao);

        if (usuario is null)
            ProcessarErro($"usuario {nomeIdentificacao} não encontrado!");

        var autenticacaoRealizada = usuario.Autenticar(senha.ToMD5());

        if (autenticacaoRealizada is false)
            ProcessarErro("Autenticacao inválida para o usuario {nomeIdentificacao}!");

        var secretKey = Environment.GetEnvironmentVariable("SecretKey") ?? _configuration["SecretKey"];
        return _tokenService.Gerar(usuario, secretKey);
    }

    private void ProcessarErro(string log)
    {
        _logger.LogWarning(log);
        throw new AutenticacaoException("Ocorreu um erro ao realizar a autenticação, verifique o usuário e senha informados!");
    }
}
