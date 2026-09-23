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
    private readonly IPropertyVisualizationRenderingService _renderingService;
    private readonly ILogger<LocalPropertyVisualizationIntakeService> _logger;

    public LocalPropertyVisualizationIntakeService(
        IWebHostEnvironment environment,
        IPropertyVisualizationRenderingService renderingService,
        ILogger<LocalPropertyVisualizationIntakeService> logger)
    {
        _environment = environment;
        _renderingService = renderingService;
        _logger = logger;
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
        await using (var destinationStream = File.Create(storagePath))
        {
            await sourceStream.CopyToAsync(destinationStream, cancellationToken);
        }

        var status = "Completed";
        string? errorMessage = null;
        string? resultExtension = null;

        try
        {
            var prompt = VisualizationPromptBuilder.Build(request.DesignOptionsJson, request.Notes);

            await using var renderSourceStream = File.OpenRead(storagePath);
            var renderResult = await _renderingService.RenderAsync(
                renderSourceStream,
                image.ContentType,
                prompt,
                cancellationToken);

            resultExtension = renderResult.ContentType switch
            {
                "image/webp" => ".webp",
                "image/jpeg" => ".jpg",
                _ => ".png"
            };

            var resultPath = Path.Combine(storageDirectory, $"{intakeId:N}-generated{resultExtension}");
            await File.WriteAllBytesAsync(resultPath, renderResult.ImageBytes, cancellationToken);
        }
        catch (VisualizationRenderException exception)
        {
            _logger.LogError(exception, "Failed to render OpenAI visualization for intake {IntakeId}.", intakeId);
            status = "Failed";
            errorMessage = exception.Message;
        }

        var metadata = new
        {
            intakeId,
            request.CustomerId,
            request.PropertyId,
            request.DesignOptionsJson,
            request.Notes,
            image.ContentType,
            image.Length,
            createdAtUtc = DateTime.UtcNow,
            status,
            errorMessage
        };

        var metadataPath = Path.Combine(storageDirectory, $"{intakeId:N}.json");
        await File.WriteAllTextAsync(
            metadataPath,
            JsonSerializer.Serialize(metadata),
            cancellationToken);

        return new PropertyVisualizationIntakeResponse
        {
            IntakeId = intakeId,
            Status = status,
            SourceUrl = $"/api/property-visualizations/{intakeId}/source",
            ResultUrl = status == "Completed" ? $"/api/property-visualizations/{intakeId}/result" : null,
            CustomerId = request.CustomerId,
            PropertyId = request.PropertyId,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
