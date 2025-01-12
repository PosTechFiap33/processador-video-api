using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessadorVideo.CrossCutting.Configurations;

namespace ProcessadorVideo.Data.Configurations;

[ExcludeFromCodeCoverage]
public static class DynamoDbConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AWSConfiguration>(configuration.GetSection("AWS"));
        // services.AddScoped<PagamentoDynamoDbContext>();
        // services.AddTransient<IPagamentoRepository, PagamentoRepository>();
        return services;
    }
}