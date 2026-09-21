using System.Text.Json;
using ExteriorServices.Api.Errors;
using ExteriorServices.Api.Models;

namespace ExteriorServices.Api.Services;

public sealed class LocalPropertyVisualizationIntakeService : IPropertyVisualizationIntakeService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private const long MaxImageSizeBytes = 10 * 1024 * 1024;
    private readonly IWebHostEnvironment _environment;

    public LocalPropertyVisualizationIntakeService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<PropertyVisualizationIntakeResponse> IntakeAsync(
        PropertyVisualizationIntakeRequest request,
        CancellationToken cancellationToken = default)
    {
        var image = request.Image;
        if (image is null || image.Length == 0)
        {
            throw new VisualizationValidationException("A property image is required.");
        }

        if (image.Length > MaxImageSizeBytes)
        {
            throw new VisualizationValidationException("The property image must be 10 MB or smaller.");
        }

        if (!AllowedContentTypes.Contains(image.ContentType))
        {
            throw new VisualizationValidationException("Only JPEG, PNG, and WebP images are supported.");
        }

        if (!string.IsNullOrWhiteSpace(request.DesignOptionsJson))
        {
            try
            {
                using var designOptions = JsonDocument.Parse(request.DesignOptionsJson);
            }
            catch (JsonException)
            {
                throw new VisualizationValidationException("Design options must be valid JSON.");
            }
        }

        var intakeId = Guid.NewGuid();
        var storageDirectory = Path.Combine(_environment.ContentRootPath, "App_Data", "visualizations");
        Directory.CreateDirectory(storageDirectory);

        var extension = image.ContentType switch
        {
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".jpg"
        };
        var storagePath = Path.Combine(storageDirectory, $"{intakeId:N}{extension}");

        await using var sourceStream = image.OpenReadStream();
        await using var destinationStream = File.Create(storagePath);
        await sourceStream.CopyToAsync(destinationStream, cancellationToken);

        var metadata = new
        {
            intakeId,
            request.CustomerId,
            request.PropertyId,
            request.DesignOptionsJson,
            request.Notes,
            image.ContentType,
            image.Length,
            createdAtUtc = DateTime.UtcNow
        };

        var metadataPath = Path.Combine(storageDirectory, $"{intakeId:N}.json");
        await File.WriteAllTextAsync(
            metadataPath,
            JsonSerializer.Serialize(metadata),
            cancellationToken);

        return new PropertyVisualizationIntakeResponse
        {
            IntakeId = intakeId,
            SourceUrl = $"/api/property-visualizations/{intakeId}/source",
            CustomerId = request.CustomerId,
            PropertyId = request.PropertyId,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
