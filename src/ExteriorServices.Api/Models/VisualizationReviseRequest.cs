using System.ComponentModel.DataAnnotations;

namespace ExteriorServices.Api.Models;

public sealed class VisualizationReviseRequest
{
    [Required]
    [MaxLength(4000)]
    public string Notes { get; init; } = string.Empty;
}
