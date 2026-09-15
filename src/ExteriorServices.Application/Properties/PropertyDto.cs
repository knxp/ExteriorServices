namespace ExteriorServices.Application.Properties;

public class PropertyDto
{
    public int Id { get; init; }

    public int CustomerId { get; init; }

    public string AddressLine1 { get; init; } = string.Empty;

    public string? AddressLine2 { get; init; }

    public string City { get; init; } = string.Empty;

    public string State { get; init; } = string.Empty;

    public string PostalCode { get; init; } = string.Empty;

    public string? PropertyType { get; init; }

    public string? Notes { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}