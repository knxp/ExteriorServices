namespace ExteriorServices.Application.Jobs;

public class JobDto
{
    public int Id { get; init; }

    public int CustomerId { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string? Phone { get; init; }

    public string AddressLine1 { get; init; } = string.Empty;

    public string City { get; init; } = string.Empty;

    public string PostalCode { get; init; } = string.Empty;

    public decimal Estimate { get; init; }

    public decimal UpFront { get; init; }

    public decimal JobTotal { get; init; }

    public DateTime ContactDate { get; init; }

    public DateTime? InstallDate { get; init; }

    public DateTime? TeardownDate { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}
