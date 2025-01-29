using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProcessadorVideo.Domain.Adapters.Repositories;
using ProcessadorVideo.Domain.DomainObjects.Exceptions;
using ProcessadorVideo.Domain.Entities;

namespace ProcessadorVideo.Identity;

public class IdentityContext : DbContext, IUnitOfWork
{
    private readonly ILogger<IdentityContext> _logger;

    public IdentityContext(DbContextOptions options,
                           ILogger<IdentityContext> logger) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
        _logger = logger;
    }

    public DbSet<Usuario> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<ValidationResult>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public async Task Commit()
    {
        try
        {
            await base.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ocorreu um erro ao commitar na base de dados: {ex.Message}");
            throw new IntegrationException("Ocorreu um erro ao efetivar a operação, tente novamente mais tarde.");
        }
    }
}
