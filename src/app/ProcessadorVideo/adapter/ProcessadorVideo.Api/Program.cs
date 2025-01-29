using Microsoft.AspNetCore.Http.Features;
using ProcessadorVideo.Infra.Configurations;
using ProcessadorVideo.Application.Configurations;
using ProcessadorVideo.Data.Configurations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProcessadorVideo.Identity.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços ao contêiner
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Api processamento de vídeos",
        Description = "Api que realiza a conversão de vídeo para imagens",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Flávio Roberto Teixeira",
            Email = "flavio.r.teixeira@outlook.com",
        }
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = 900 * 1024 * 1024; // 900 MB limit (ajuste conforme necessário)
    options.MultipartBodyLengthLimit = 900 * 1024 * 1024; // 900 MB limit (ajuste conforme necessário)
});

// Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Permite qualquer origem (domínio)
              .AllowAnyMethod() // Permite qualquer método HTTP (GET, POST, etc.)
              .AllowAnyHeader(); // Permite qualquer cabeçalho
    });
});

builder.Services.AddInfra(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddDataConfiguration(builder.Configuration);
builder.Services.AddIdentity(builder.Configuration);

builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());

var app = builder.Build();

// Configura o pipeline de requisição HTTP
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
    options.RoutePrefix = "swagger"; // Swagger será acessível em /swagger
});

app.UseHttpsRedirection();

app.UseStaticFiles(); // Adiciona suporte para arquivos estáticos

// Ativa o CORS
app.UseCors("AllowAll"); // Aplica a política de CORS

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
});

app.Run();
