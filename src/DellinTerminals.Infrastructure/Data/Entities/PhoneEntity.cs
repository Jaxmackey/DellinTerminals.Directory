namespace DellinTerminals.Infrastructure.Data.Entities;

public class PhoneEntity
{
    public int Id { get; set; }
    public int OfficeId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Additional { get; set; }
    
    public OfficeEntity Office { get; set; } = null!;
}