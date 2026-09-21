using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ExteriorServices.Api.Models;

public sealed class PropertyVisualizationIntakeRequest
{
    [Required]
    public IFormFile? Image { get; init; }

    public int? CustomerId { get; init; }

    public int? PropertyId { get; init; }

    public string? DesignOptionsJson { get; init; }

    [MaxLength(4000)]
    public string? Notes { get; init; }
}
