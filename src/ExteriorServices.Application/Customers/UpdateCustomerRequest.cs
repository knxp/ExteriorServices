using System.ComponentModel.DataAnnotations;

namespace ExteriorServices.Application.Customers;

public class UpdateCustomerRequest
{
    [Required]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    public string LastName { get; init; } = string.Empty;

    public string? Phone { get; init; }

    [EmailAddress]
    public string? Email { get; init; }

    public bool IsActive { get; init; }
}