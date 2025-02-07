using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Identity.Repositories;
using ProcessadorVideo.Identity.Services;

namespace ProcessadorVideo.Identity.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuração do DbContext
            services.AddScoped<IdentityContext>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ITokenService, TokenService>();

            var connectionEnv = "ConnectionString";
            var connectionString = Environment.GetEnvironmentVariable(connectionEnv) ?? configuration[connectionEnv];

            //TODO avalidar melhor depois pra resolver na fixture de teste
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Testing")
                services.AddDbContext<IdentityContext>(options =>
                    options.UseNpgsql(connectionString));

            services.ConfigureMigrationDatabase();

            return services;
        }

        private static void ConfigureMigrationDatabase(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            try
            {
                var dbContext = serviceProvider.GetRequiredService<IdentityContext>();
                dbContext.Database.Migrate(); // Aplica migrações automaticamente
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<IdentityContext>>();
                logger.LogError(ex, "Ocorreu um erro ao executar a migration do banco de dados!");
            }
        }
    }
}
