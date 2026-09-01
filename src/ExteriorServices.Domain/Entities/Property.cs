using System.Text.Json.Serialization;

namespace ExteriorServices.Domain.Entities;

public class Property
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string? PropertyType { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [JsonIgnore]
    public Customer? Customer { get; set; }
}