using DellinTerminals.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DellinTerminals.Infrastructure.Configurations;

public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
{
    public void Configure(EntityTypeBuilder<Phone> builder)
    {
        builder.ToTable("phones");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(50).HasColumnName("phone_number");
        builder.Property(p => p.Additional).HasMaxLength(100).HasColumnName("additional");
        builder.Property(p => p.OfficeId).HasColumnName("office_id");
        
        builder.HasIndex(p => p.OfficeId).HasDatabaseName("ix_phones_office_id");
    }
}