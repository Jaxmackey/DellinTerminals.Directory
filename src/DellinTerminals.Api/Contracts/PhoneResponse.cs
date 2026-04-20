namespace DellinTerminals.Api.Contracts;

public class PhoneResponse
{
    public string PhoneNumber { get; init; } = string.Empty;
    public string? Additional { get; init; }
}