using BlockEstate.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BlockEstate.Api.Data;

public sealed class RegistryDbContext(DbContextOptions<RegistryDbContext> options) : DbContext(options)
{
    public DbSet<PropertyRecord> Properties => Set<PropertyRecord>();
    public DbSet<RegistrationApplication> Applications => Set<RegistrationApplication>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var property = modelBuilder.Entity<PropertyRecord>();
        property.ToTable("PropertyRecords");
        property.HasKey(x => x.Id);
        property.HasIndex(x => x.PropertyCode).IsUnique();
        property.HasIndex(x => x.ApplicationCode);
        property.Property(x => x.PropertyCode).HasMaxLength(40).IsRequired();
        property.Property(x => x.Title).HasMaxLength(180).IsRequired();
        property.Property(x => x.PropertyType).HasMaxLength(80).IsRequired();
        property.Property(x => x.Location).HasMaxLength(240).IsRequired();
        property.Property(x => x.RecordedOwner).HasMaxLength(160).IsRequired();
        property.Property(x => x.Area).HasMaxLength(80).IsRequired();
        property.Property(x => x.Status).HasMaxLength(40).IsRequired();
        property.Property(x => x.RiskLevel).HasMaxLength(30).IsRequired();
        property.Property(x => x.ApplicationCode).HasMaxLength(40).IsRequired();

        var application = modelBuilder.Entity<RegistrationApplication>();
        application.ToTable("RegistrationApplications");
        application.HasKey(x => x.Id);
        application.HasIndex(x => x.ApplicationCode).IsUnique();
        application.HasIndex(x => x.Status);
        application.Property(x => x.ApplicationCode).HasMaxLength(40).IsRequired();
        application.Property(x => x.PropertyCode).HasMaxLength(40).IsRequired();
        application.Property(x => x.ApplicantName).HasMaxLength(160).IsRequired();
        application.Property(x => x.WorkflowType).HasMaxLength(80).IsRequired();
        application.Property(x => x.Status).HasMaxLength(40).IsRequired();
        application.Property(x => x.Priority).HasMaxLength(30).IsRequired();

        var audit = modelBuilder.Entity<AuditEvent>();
        audit.ToTable("AuditEvents");
        audit.HasKey(x => x.Id);
        audit.HasIndex(x => x.CreatedAt);
        audit.Property(x => x.ActorLabel).HasMaxLength(160).IsRequired();
        audit.Property(x => x.Action).HasMaxLength(240).IsRequired();
        audit.Property(x => x.Target).HasMaxLength(80).IsRequired();
        audit.Property(x => x.Kind).HasMaxLength(50).IsRequired();
        audit.Property(x => x.Details).HasMaxLength(1000).IsRequired();
    }
}
