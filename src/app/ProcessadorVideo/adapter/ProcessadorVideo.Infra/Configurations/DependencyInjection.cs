using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.Domain.Adapters.MessageBus;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Infra.Messaging;
using ProcessadorVideo.Infra.Messaging.Workers;
using ProcessadorVideo.Infra.Services;

namespace ProcessadorVideo.Infra.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AWSConfiguration>(configuration.GetSection("AWS"));

        services.AddScoped<IMessageBus, SqsMessageBus>();

        //TODO: avaliar se ta ok singleton
        services.AddSingleton<IVideoService, VideoService>();
        services.AddSingleton<IFileStorageService, BucketS3StorageService>();

        services.AddHostedService<ConverterVideoParaImagemMessagingWorker>();

        return services;
    }
}
