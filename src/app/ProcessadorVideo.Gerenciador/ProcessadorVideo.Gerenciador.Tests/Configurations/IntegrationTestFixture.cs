using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProcessadorVideo.Gerenciador.Tests.Configurations;

public class IntegrationTestFixture : IDisposable
{
    public WebApplicationFactory<Program> Factory { get; private set; }

    public IntegrationTestFixture()
    {
        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
           {
               builder.ConfigureAppConfiguration((context, config) =>
                  {
                      context.HostingEnvironment.EnvironmentName = "Testing";
                  });
           }); ;

    }
    public HttpClient GerarHttpClient()
    {
        var chave = "chave_token_secreta_muito_segura_de_128_bits!";
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("SecretKey", chave);


        var Client = Factory.CreateClient();
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GerarTokenJwt(chave)}");

        return Client;
    }

    public void AdicionarDependencia(Action<IServiceCollection> configureServices = null)
    {
        Factory = Factory.WithWebHostBuilder(builder =>
           {
               if (configureServices != null)
                   builder.ConfigureServices(configureServices);
           });
    }

    public IServiceCollection RemoverServicoInjetado<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
        return services;
    }

    private string GerarTokenJwt(string chaveSecreta)
    {
        var key = Encoding.UTF8.GetBytes(chaveSecreta);

        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim>
            {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "administrador")
            }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }


    public void Dispose()
    {
        Factory.Dispose();
    }
}
