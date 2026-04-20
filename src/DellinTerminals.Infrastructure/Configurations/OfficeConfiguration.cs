using DellinTerminals.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DellinTerminals.Infrastructure.Configurations;

public class OfficeConfiguration : IEntityTypeConfiguration<Office>
{
    public void Configure(EntityTypeBuilder<Office> builder)
    {
        builder.ToTable("offices");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Code).HasMaxLength(50).HasColumnName("code");
        builder.Property(o => o.CityCode).HasColumnName("city_code");
        builder.Property(o => o.Uuid).HasMaxLength(100).HasColumnName("uuid");
        builder.Property(o => o.Type).HasConversion<string>().HasColumnName("type");
        builder.Property(o => o.CountryCode).IsRequired().HasMaxLength(10).HasColumnName("country_code");
        
        // Owned type Coordinates
        builder.OwnsOne(o => o.Coordinates, c =>
        {
            c.Property(x => x.Latitude).HasColumnName("latitude");
            c.Property(x => x.Longitude).HasColumnName("longitude");
        });
        
        builder.Property(o => o.AddressRegion).HasMaxLength(200).HasColumnName("address_region");
        builder.Property(o => o.AddressCity).HasMaxLength(200).HasColumnName("address_city");
        builder.Property(o => o.AddressStreet).HasMaxLength(200).HasColumnName("address_street");
        builder.Property(o => o.AddressHouseNumber).HasMaxLength(50).HasColumnName("address_house_number");
        builder.Property(o => o.AddressApartment).HasColumnName("address_apartment");
        builder.Property(o => o.WorkTime).HasMaxLength(100).HasColumnName("work_time");
        
        // Поле для быстрого поиска (нормализованное название города)
        builder.Property(o => o.NormalizedCityName)
            .HasMaxLength(200)
            .HasColumnName("normalized_city_name")
            .HasDefaultValue("");
        
        // Индексы по ТЗ
        builder.HasIndex(o => new { o.NormalizedCityName, o.AddressRegion })
            .HasDatabaseName("ix_offices_city_region");
        
        builder.HasIndex(o => o.CityCode).HasDatabaseName("ix_offices_city_code");
        
        // Отношение к Phone
        builder.HasMany(o => o.Phones)
            .WithOne(p => p.Office)
            .HasForeignKey(p => p.OfficeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}