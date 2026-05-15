using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
using Domain.ValueObjects;

namespace Infrastructure.PostgreSQL.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, v => EntityId.Create(v))
            .ValueGeneratedNever();

        builder.Property(d => d.ParentId)
            .HasColumnName("parent_id")
            .HasColumnType("uuid")
            .HasConversion(pid => pid.Value, v => ParentId.Create(v));

        builder.Property(d => d.Name)
            .HasColumnName("department_name")
            .HasColumnType("varchar(255)")
            .HasConversion(name => name.Value, v => Domain.ValueObjects.Name.Create(v))
            .IsRequired();

        builder.HasIndex(d => d.Name)
            .IsUnique();

        builder.Property(d => d.Identifier)
            .HasColumnName("department_identifier")
            .HasColumnType("text")
            .HasConversion(id => id.Value, v => Domain.ValueObjects.Identifier.Create(v))
            .IsRequired();

        builder.Property(d => d.Path)
            .HasColumnName("department_path")
            .HasColumnType("text")
            .HasConversion(path => path.Value, v => HierarchyPath.Create(v))
            .IsRequired();

        builder.Property(d => d.Depth)
            .HasColumnName("department_depth")
            .HasColumnType("int")
            .HasConversion(depth => depth.Value, v => Domain.ValueObjects.Depth.Create(v))
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .HasConversion(dt => dt.Value, v => UtcDateTime.Create(v))
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp")
            .HasConversion(dt => dt.Value, v => UtcDateTime.Create(v));

        builder.Property<DateTime?>("DeletedAt")
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp");
    }
}