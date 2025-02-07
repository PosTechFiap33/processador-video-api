using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using ProcessadorVideo.Identity.Api.DTOs;
using ProcessadorVideo.Identity.Tests.Configurations;
using TechTalk.SpecFlow;
using Xunit;

namespace ProcessadorVideo.Identity.Tests.Features;

public class TokenModel
{
    [JsonPropertyName("token")]
    public string Token { get; set; }
}

public class UsaurioCadastradoModel
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("nomeIdentificacao")]
    public string NomeIdentificacao { get; set; }
}

[Binding]
public class UsuarioStepDefinitions : IClassFixture<IntegrationTestFixture>
{
    private readonly UsuarioDTO _usuario;
    private readonly HttpClient _client;
    private HttpResponseMessage _response;


    public UsuarioStepDefinitions(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
        _usuario = new UsuarioDTO();
    }

    [Given(@"que eu informe o nome de identificacao ""(.*)""")]
    public void Givenqueeuinformeonomedeidentificacao(string nomeUsuario)
    {
        _usuario.Usuario = nomeUsuario;
    }

    [Given(@"o email ""(.*)""")]
    public void Givenoemail(string email)
    {
        _usuario.Email = email;
    }

    [Given(@"a senha ""(.*)""")]
    public void Givenasenha(string senha)
    {
        _usuario.Senha = senha;
    }

    [When(@"for feita a requisição para a rota de cadastro")]
    public async Task Whenforfeitaarequisiçãoparaarotadecadastro()
    {
        _response = await _client.PostAsJsonAsync("Usuario", _usuario);
    }

    [Then(@"devera ser retornado o status (.*)")]
    public void Thendeverserretornadoostatus(int status)
    {
        _response.StatusCode.Should().Be((HttpStatusCode)status);
    }

    [Then(@"o id do usuario deve ser válido")]
    public async Task Givenoiddousuariodeveserválido()
    {
        var dados = await _response.Content.ReadAsStringAsync();
        var usuario = JsonSerializer.Deserialize<UsaurioCadastradoModel>(dados);
        usuario.Id.Should().NotBe(Guid.Empty);
        usuario.NomeIdentificacao.Should().Be(_usuario.Usuario);
    }

    [When(@"for realizada uma autenticacao com o usuario e senha informados")]
    public async Task Whenforrealizadaumaautenticacaocomousuarioesenhainformados()
    {
        _response = await _client.PostAsJsonAsync("Autenticacao", new AutenticacaoDTO
        {
            Usuario = _usuario.Usuario,
            Senha = _usuario.Senha
        });
    }

    [Then(@"devera ser retornado o token")]
    public async Task Givendeveraserretornadootoken()
    {
        var dados = await _response.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<TokenModel>(dados);
        response?.Token.Should().NotBeNullOrEmpty();
    }

}