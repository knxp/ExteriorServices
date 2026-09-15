using System.ComponentModel.DataAnnotations;

namespace ExteriorServices.Application.Properties;

public class UpdatePropertyRequest
{
    [Required]
    public string AddressLine1 { get; init; } = string.Empty;

    public string? AddressLine2 { get; init; }

    [Required]
    public string City { get; init; } = string.Empty;

    [Required]
    public string State { get; init; } = string.Empty;

    [Required]
    public string PostalCode { get; init; } = string.Empty;

    public string? PropertyType { get; init; }

    public string? Notes { get; init; }
}