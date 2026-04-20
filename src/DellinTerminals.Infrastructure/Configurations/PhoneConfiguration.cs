using DellinTerminals.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DellinTerminals.Infrastructure.Configurations;

public class PhoneConfiguration : IEntityTypeConfiguration<PhoneEntity>
{
    public void Configure(EntityTypeBuilder<PhoneEntity> builder)
    {
        builder.ToTable("phones");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(50).HasColumnName("phone_number");
        builder.Property(p => p.Additional).HasMaxLength(100).HasColumnName("additional");
    }
}