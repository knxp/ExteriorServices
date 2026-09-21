namespace ExteriorServices.Web.Models;

public sealed class VisualizationImageSummary
{
    public Guid IntakeId { get; set; }

    public string SourceUrl { get; set; } = string.Empty;

    public int? CustomerId { get; set; }

    public int? PropertyId { get; set; }

    public string ContentType { get; set; } = string.Empty;

    public long Size { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
