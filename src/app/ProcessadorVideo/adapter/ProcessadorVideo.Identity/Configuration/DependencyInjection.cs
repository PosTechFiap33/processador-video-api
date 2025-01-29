using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Identity.Repositories;
using ProcessadorVideo.Identity.Services;

namespace ProcessadorVideo.Identity.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration){
        services.AddScoped<IdentityContext>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ITokenService, TokenService>();

        var connectionEnv = "ConnectionString";
        var connectionString = Environment.GetEnvironmentVariable(connectionEnv) ?? configuration[connectionEnv];
        services.AddDbContext<IdentityContext>(options => options.UseNpgsql(connectionString));

        services.ConfigureMigrationDatabase();

        return services;
    }

    public static void ConfigureMigrationDatabase(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        try
        {
            var dbContext = serviceProvider.GetRequiredService<IdentityContext>();
            dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<IdentityContext>>();
            logger.LogError(ex, "Ocorreu um erro ao executar a migration do banco de dados!");
        }
    }
}
