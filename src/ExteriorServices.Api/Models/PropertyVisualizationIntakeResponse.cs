namespace ExteriorServices.Api.Models;

public sealed class PropertyVisualizationIntakeResponse
{
    public Guid IntakeId { get; init; }

    public string Status { get; init; } = "StoredForTesting";

    public string SourceUrl { get; init; } = string.Empty;

    public int? CustomerId { get; init; }

    public int? PropertyId { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}
