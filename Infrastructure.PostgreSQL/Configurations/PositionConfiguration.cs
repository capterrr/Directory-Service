using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
using Domain.ValueObjects;

namespace Infrastructure.PostgreSQL.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, v => EntityId.Create(v))
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasColumnName("position_name")
            .HasColumnType("varchar(255)")
            .HasConversion(name => name.Value, v => Domain.ValueObjects.Name.Create(v))
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasColumnType("text")
            .HasConversion(desc => desc.Value, v => Domain.ValueObjects.Description.Create(v))
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .HasConversion(dt => dt.Value, v => UtcDateTime.Create(v))
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp")
            .HasConversion(dt => dt.Value, v => UtcDateTime.Create(v));

        builder.Property<DateTime?>("DeletedAt")
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp");
    }
}