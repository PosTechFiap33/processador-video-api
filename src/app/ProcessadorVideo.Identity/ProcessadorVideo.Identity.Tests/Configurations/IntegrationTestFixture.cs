using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using ProcessadorVideo.Domain.Entities;
using ProcessadorVideo.CrossCutting.Extensions;

namespace ProcessadorVideo.Identity.Tests.Configurations;

public class IntegrationTestFixture : IDisposable
{
    public WebApplicationFactory<Program> Factory { get; }
    public HttpClient Client { get; }
    public IdentityContext context { get; private set; }

    public IntegrationTestFixture()
    {
        // Definindo a variável de ambiente
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("SecretKey", "chave_token_secreta_muito_segura_de_128_bits!");

        Factory = new WebApplicationFactory<Program>()
         .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                   {
                       context.HostingEnvironment.EnvironmentName = "Testing";
                   });

                builder.ConfigureServices(async services =>
                {
                    // Remove o contexto de banco de dados existente, se houver
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<IdentityContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<IdentityContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });

                    var serviceProvider = services.BuildServiceProvider();

                    context = serviceProvider.GetService<IdentityContext>();
                    context.Database.EnsureCreated();
                    await SeedDatabase();
                });
            });
 
        Client = Factory.CreateClient();
    }

    public async Task TestarRequisicaoComErro(HttpResponseMessage response, List<string> erros)
    {
        var dados = await response.Content.ReadAsStringAsync();
        var errorDetail = JsonSerializer.Deserialize<ValidationProblemDetails>(dados);

        new ValidationProblemDetails(new Dictionary<string, string[]> {
                { "Mensagens", erros.ToArray() }
            });

        errorDetail.Errors["Mensagens"].Should().BeEquivalentTo(erros);
    }

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
    }

    private async Task SeedDatabase()
    {
        context.Usuario.Add(new Usuario("vader", "darkside".ToMD5(), "teste@email.com", Perfil.Administrador));
        await context.SaveChangesAsync();
    }
}
