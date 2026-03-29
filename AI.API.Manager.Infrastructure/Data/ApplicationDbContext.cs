using AI.API.Manager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AI.API.Manager.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<AIProvider> AIProviders => Set<AIProvider>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<RequestLog> RequestLogs => Set<RequestLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ConfigureTenant(modelBuilder);
        ConfigureAIProvider(modelBuilder);
        ConfigureApiKey(modelBuilder);
        ConfigureRequestLog(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Tenant || e.Entity is AIProvider || e.Entity is ApiKey)
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        var utcNow = DateTime.UtcNow;

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property("CreatedAt").CurrentValue = utcNow;
                entityEntry.Property("UpdatedAt").CurrentValue = utcNow;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property("UpdatedAt").CurrentValue = utcNow;
            }
        }
    }

    private static void ConfigureTenant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsActive).IsRequired();

            // Shadow properties for timestamps
            entity.Property<DateTime>("CreatedAt").IsRequired();
            entity.Property<DateTime>("UpdatedAt").IsRequired();

            entity.HasIndex(e => e.Name).IsUnique();
        });
    }

    private static void ConfigureAIProvider(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AIProvider>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.BaseUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ProviderType).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.MaxConcurrentRequests).IsRequired();
            entity.Property(e => e.RequestTimeoutSeconds).IsRequired();

            // Shadow properties for timestamps
            entity.Property<DateTime>("CreatedAt").IsRequired();
            entity.Property<DateTime>("UpdatedAt").IsRequired();

            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.ProviderType);
        });
    }

    private static void ConfigureApiKey(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.ProviderId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.KeyValue).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.RateLimitPerMinute).IsRequired();
            entity.Property(e => e.ExpiresAt);

            // Shadow properties for timestamps
            entity.Property<DateTime>("CreatedAt").IsRequired();
            entity.Property<DateTime>("UpdatedAt").IsRequired();

            entity.HasIndex(e => new { e.TenantId, e.ProviderId, e.Name }).IsUnique();
            entity.HasIndex(e => e.KeyValue).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.ProviderId);

            entity.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<AIProvider>()
                .WithMany()
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureRequestLog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RequestLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.ProviderId).IsRequired();
            entity.Property(e => e.ApiKeyId).IsRequired();
            entity.Property(e => e.Endpoint).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Method).IsRequired().HasMaxLength(10);
            entity.Property(e => e.StatusCode).IsRequired();
            entity.Property(e => e.DurationMs).IsRequired();
            entity.Property(e => e.RequestSizeBytes);
            entity.Property(e => e.ResponseSizeBytes);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.ProviderId);
            entity.HasIndex(e => e.ApiKeyId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.StatusCode);

            entity.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<AIProvider>()
                .WithMany()
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<ApiKey>()
                .WithMany()
                .HasForeignKey(e => e.ApiKeyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}