using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.PostgreSQL.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_positions");

        builder.HasKey(dp => new { dp.PositionId, dp.DepartmentId });

        builder.Property(dp => dp.PositionId)
            .HasColumnName("position_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(dp => dp.DepartmentId)
            .HasColumnName("department_id")
            .HasColumnType("uuid")
            .IsRequired();
    }
}