using BlockEstate.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BlockEstate.Api.Data;

public sealed class RegistryDbContext(DbContextOptions<RegistryDbContext> options) : DbContext(options)
{
    public DbSet<PropertyRecord> Properties => Set<PropertyRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var property = modelBuilder.Entity<PropertyRecord>();
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
    }
}
