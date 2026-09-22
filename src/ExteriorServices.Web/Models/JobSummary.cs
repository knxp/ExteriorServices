namespace ExteriorServices.Web.Models;

public class JobSummary
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public decimal Estimate { get; set; }

    public decimal UpFront { get; set; }

    public decimal JobTotal { get; set; }

    public DateTime ContactDate { get; set; }

    public DateTime? InstallDate { get; set; }

    public DateTime? TeardownDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
