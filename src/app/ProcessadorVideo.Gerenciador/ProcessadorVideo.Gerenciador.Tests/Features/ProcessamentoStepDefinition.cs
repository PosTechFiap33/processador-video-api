using System;
using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProcessadorVideo.Application.DTOs;
using ProcessadorVideo.Domain.Adapters.MessageBus.Messages;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Domain.Entities;
using ProcessadorVideo.Gerenciador.Tests.Configurations;
using TechTalk.SpecFlow;
using Xunit;

namespace ProcessadorVideo.Gerenciador.Tests.Features;

public class ProcessamentoModel
{
    public string id { get; set; }
    public int status { get; set; }
    public List<string> Mensagens { get; set; }
}

[Binding]
public class ProcessamentoStepDefinition : IClassFixture<IntegrationTestFixture>
{
    private HttpClient _client;
    private HttpResponseMessage _response;
    private readonly IntegrationTestFixture _fixture;
    private readonly ProcessamentoVideo _processamentoMock;

    public ProcessamentoStepDefinition(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _processamentoMock = new ProcessamentoVideo(Guid.NewGuid(),
                                   Guid.NewGuid(),
                                   new Arquivo("teste", "teste"),
                                   StatusProcessamento.Processado,
                                   DateTime.Now,
                                   new List<string> { "teste" });
    }

    [Given(@"que tenha processsamentos cadastrados")]
    public void Givenquetenhaprocesssamentoscadastrados()
    {
        var processamentos = new List<ProcessamentoVideo> {
            _processamentoMock
        };

        var processamentoVideoRepositoryMock = new Mock<IProcessamentoVideoRepository>();
        processamentoVideoRepositoryMock.Setup(x => x.ListarPorUsuario(It.IsAny<Guid>()))
                                        .ReturnsAsync(processamentos);

        _fixture.AdicionarDependencia(s =>
        {
            s.AddSingleton(s => processamentoVideoRepositoryMock.Object);
        });

        _client = _fixture.GerarHttpClient();
    }


    [Given(@"que tenha processsamentos concluidos cadastrados")]
    public void Givenquetenhaprocesssamentosconcluidoscadastrados()
    {
        var processamentoVideoRepositoryMock = new Mock<IProcessamentoVideoRepository>();
        processamentoVideoRepositoryMock.Setup(x => x.Consultar(It.Is<Guid>(id => id == _processamentoMock.Id)))
                                        .ReturnsAsync(_processamentoMock);

        var fileStorageServiceMock = new Mock<IFileStorageService>();
        fileStorageServiceMock.Setup(x => x.Ler(It.Is<string>(s => s == _processamentoMock.ArquivoDownload.Diretorio),
                                                It.Is<string>(s => s == _processamentoMock.ArquivoDownload.Nome)))
                              .ReturnsAsync([1, 2, 3]);

        _fixture.AdicionarDependencia(s =>
         {
             s.AddSingleton(s => processamentoVideoRepositoryMock.Object);
             s.AddSingleton(s => fileStorageServiceMock.Object);
         });

        _client = _fixture.GerarHttpClient();
    }

    [When(@"for feita a requisição para a rota listagem de processamento")]
    public async Task Whenforfeitaarequisiçãoparaarotalistagemdeprocessamento()
    {
        _response = await _client.GetAsync("ProcessamentoVideo");
    }

    [Then(@"devera ser retornado o status (.*)")]
    public void Thendeveraserretornadoostatus(decimal status)
    {
        _response.StatusCode.Should().Be((HttpStatusCode)status);
    }

    [Then(@"devera ser retornado a lista de processamentos")]
    public async Task Givendeveraserretornadoalistadeprocessamentos()
    {
        var dados = await _response.Content.ReadAsStringAsync();

        var response = JsonSerializer.Deserialize<List<ProcessamentoModel>>(dados, new JsonSerializerOptions
        {
            IncludeFields = true
        });

        response.Should().Contain(x => x.status == (int)StatusProcessamento.Processado &&
                                       x.Mensagens.Contains("teste") &&
                                       !string.IsNullOrEmpty(x.id));
    }

    [When(@"for feita a requisição para a rota download de processamento")]
    public async Task Whenforfeitaarequisiçãoparaarotadownloaddeprocessamento()
    {
        _response = await _client.GetAsync($"ProcessamentoVideo/{_processamentoMock.Id}/download");
    }

    [Then(@"devera ser retornado o binario do arquivo zip")]
    public async Task Givendeveraserretornadoobinariodoarquivozip()
    {
        _response.Content.Headers.ContentType.MediaType.Should().Be("application/zip");
        var dados = await _response.Content.ReadAsByteArrayAsync();
        dados.Should().NotBeNullOrEmpty();
    }

}