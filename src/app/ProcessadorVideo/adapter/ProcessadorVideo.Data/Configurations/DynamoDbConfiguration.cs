using System.Diagnostics.CodeAnalysis;
using Amazon.DynamoDBv2;
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
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.Configure<AWSConfiguration>(configuration.GetSection("AWS"));
        services.AddAWSService<IAmazonDynamoDB>();

        services.AddScoped<ProcessamentoVideoDynamoContext>();
        services.AddTransient<IProcessamentoVideoRepository, ProcessamentoVideoRepository>();
        return services;
    }
}