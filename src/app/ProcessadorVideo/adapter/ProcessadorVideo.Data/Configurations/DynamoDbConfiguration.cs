using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.Data.Repositories;
using ProcessadorVideo.Domain.Adapters.Repositories;

namespace ProcessadorVideo.Data.Configurations;

[ExcludeFromCodeCoverage]
public static class DynamoDbConfiguration
{
    public static IServiceCollection AddDataConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AWSConfiguration>(configuration.GetSection("AWS"));
        services.AddScoped<ProcessamentoVideoDynamoContext>();
        services.AddTransient<IProcessamentoPedidoRepository, ProcessamentoPedidoRepository>();
        return services;
    }
}