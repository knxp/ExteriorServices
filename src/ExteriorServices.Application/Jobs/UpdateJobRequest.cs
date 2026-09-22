using System.ComponentModel.DataAnnotations;

namespace ExteriorServices.Application.Jobs;

public class UpdateJobRequest
{
    [Required]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    public string LastName { get; init; } = string.Empty;

    public string? Phone { get; init; }

    [Required]
    public string AddressLine1 { get; init; } = string.Empty;

    [Required]
    public string City { get; init; } = string.Empty;

    [Required]
    public string PostalCode { get; init; } = string.Empty;

    public decimal Estimate { get; init; }

    public decimal UpFront { get; init; }

    public decimal JobTotal { get; init; }

    [Required]
    public DateTime ContactDate { get; init; }

    public DateTime? InstallDate { get; init; }

    public DateTime? TeardownDate { get; init; }
}
