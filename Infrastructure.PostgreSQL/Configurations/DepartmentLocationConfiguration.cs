using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.PostgreSQL.Configurations;

public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_locations");

        builder.HasKey(dl => new { dl.LocationId, dl.DepartmentId });

        builder.Property(dl => dl.LocationId)
            .HasColumnName("location_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(dl => dl.DepartmentId)
            .HasColumnName("department_id")
            .HasColumnType("uuid")
            .IsRequired();
    }
}