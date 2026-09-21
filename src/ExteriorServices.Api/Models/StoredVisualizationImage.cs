namespace ExteriorServices.Api.Models;

public sealed class StoredVisualizationImage
{
    public Guid IntakeId { get; init; }

    public string SourceUrl { get; init; } = string.Empty;

    public int? CustomerId { get; init; }

    public int? PropertyId { get; init; }

    public string ContentType { get; init; } = string.Empty;

    public long Size { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}
