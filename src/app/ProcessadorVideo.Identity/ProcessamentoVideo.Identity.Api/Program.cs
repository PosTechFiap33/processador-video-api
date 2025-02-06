using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProcessadorVideo.Identity.Api.UseCases;
using ProcessadorVideo.Identity.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAutenticarUseCase, AutenticarUseCase>();
builder.Services.AddScoped<ICadastrarUsuarioUseCase, CadastrarUsuarioUseCase>();
builder.Services.AddData(builder.Configuration);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Permite qualquer origem (domínio)
              .AllowAnyMethod() // Permite qualquer método HTTP (GET, POST, etc.)
              .AllowAnyHeader(); // Permite qualquer cabeçalho
    });
});

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

app.UseAuthentication();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
});

app.Run();
