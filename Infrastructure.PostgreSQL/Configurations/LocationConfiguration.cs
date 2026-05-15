using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
using Domain.ValueObjects;

namespace Infrastructure.PostgreSQL.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, v => EntityId.Create(v))
            .ValueGeneratedNever();

        builder.Property(l => l.Name)
            .HasColumnName("location_name")
            .HasColumnType("varchar(255)")
            .HasConversion(name => name.Value, v => Domain.ValueObjects.Name.Create(v))
            .IsRequired();

        builder.Property(l => l.Address)
            .HasColumnName("location_address")
            .HasColumnType("jsonb")
            .HasConversion(addr => addr.Value, v => Domain.ValueObjects.Address.Create(v))
            .IsRequired();

        builder.Property(l => l.Timezone)
            .HasColumnName("iana_time_zone")
            .HasColumnType("varchar(255)")
            .HasConversion(tz => tz.Value, v => Domain.ValueObjects.Timezone.Create(v))
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .HasConversion(dt => dt.Value, v => UtcDateTime.Create(v))
            .IsRequired();

        builder.Property(l => l.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp")
            .HasConversion(dt => dt.Value, v => UtcDateTime.Create(v));

        builder.Property<DateTime?>("DeletedAt")
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp");
    }
}