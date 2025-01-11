using Microsoft.Extensions.DependencyInjection;
using ProcessadorVideo.Domain.Services;
using ProcessadorVideo.Infra.Services;

namespace ProcessadorVideo.Infra.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services)
    {
        services.AddScoped<IVideoService, VideoService>();
        return services;
    }
}
