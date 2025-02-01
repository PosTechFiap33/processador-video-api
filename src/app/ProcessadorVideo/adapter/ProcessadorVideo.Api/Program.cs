using Microsoft.AspNetCore.Http.Features;
using ProcessadorVideo.Infra.Configurations;
using ProcessadorVideo.Application.Configurations;
using ProcessadorVideo.Data.Configurations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProcessadorVideo.Identity.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Entre com o seu Bearer token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
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


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"]);

        options.RequireHttpsMetadata = false;  // Para desenvolvimento, desabilitar HTTPS
        options.SaveToken = true;  // Salvar o token no contexto da requisição
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false, // Validação do emissor (Issuer)
                                    //      ValidIssuer = "your-issuer",  // Substitua com o seu emissor
            ValidateAudience = false, // Validação do público (Audience)
                                      //   ValidAudience = "your-audience",  // Substitua com seu público
            ValidateLifetime = true,  // Verifica se o token expirou
            ClockSkew = TimeSpan.Zero // Remover o atraso da verificação de expiração
        };

        // Habilitar log detalhado para depuração
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully.");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("administrador", policy =>
        policy.RequireRole("administrador"));
});

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

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
});

app.Run();
