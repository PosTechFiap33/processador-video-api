using ProcessadorVideo.Conversor.Worker;
using ProcessadorVideo.CrossCutting.Configurations;
using ProcessadorVideo.Domain.Adapters.Services;
using ProcessadorVideo.Infra.Configurations;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<AWSConfiguration>(builder.Configuration.GetSection("AWS"));
builder.Services.AddInfra(builder.Configuration);
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddHostedService<ProcessarVideoMessagingWorker>();

var host = builder.Build();
host.Run();
