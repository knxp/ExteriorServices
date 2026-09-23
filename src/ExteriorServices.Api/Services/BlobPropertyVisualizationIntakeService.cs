using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ExteriorServices.Api.Errors;
using ExteriorServices.Api.Models;

namespace ExteriorServices.Api.Services;

// Blob-storage equivalent of LocalPropertyVisualizationIntakeService - writes the same
// source/{generated}/metadata blobs to Azure Blob Storage instead of local disk.
public sealed class BlobPropertyVisualizationIntakeService : IPropertyVisualizationIntakeService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private const long MaxImageSizeBytes = 10 * 1024 * 1024;
    private readonly BlobContainerClient _containerClient;
    private readonly IPropertyVisualizationRenderingService _renderingService;
    private readonly ILogger<BlobPropertyVisualizationIntakeService> _logger;

    public BlobPropertyVisualizationIntakeService(
        BlobContainerClient containerClient,
        IPropertyVisualizationRenderingService renderingService,
        ILogger<BlobPropertyVisualizationIntakeService> logger)
    {
        _containerClient = containerClient;
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
        var extension = image.ContentType switch
        {
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => ".jpg"
        };
        var sourceBlobName = $"{intakeId:N}{extension}";

        await using (var sourceStream = image.OpenReadStream())
        {
            await UploadAsync(sourceBlobName, sourceStream, image.ContentType, cancellationToken);
        }

        var status = "Completed";
        string? errorMessage = null;

        try
        {
            var prompt = VisualizationPromptBuilder.Build(request.DesignOptionsJson, request.Notes);

            await using var renderSourceStream = image.OpenReadStream();
            var renderResult = await _renderingService.RenderAsync(
                renderSourceStream,
                image.ContentType,
                prompt,
                cancellationToken);

            var resultExtension = renderResult.ContentType switch
            {
                "image/webp" => ".webp",
                "image/jpeg" => ".jpg",
                _ => ".png"
            };

            var resultBlobName = $"{intakeId:N}-generated{resultExtension}";
            using var resultStream = new MemoryStream(renderResult.ImageBytes);
            await UploadAsync(resultBlobName, resultStream, renderResult.ContentType, cancellationToken);
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

        var metadataBlobName = $"{intakeId:N}.json";
        using var metadataStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(metadata)));
        await UploadAsync(metadataBlobName, metadataStream, "application/json", cancellationToken);

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

    private async Task UploadAsync(string blobName, Stream content, string contentType, CancellationToken cancellationToken)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(
            content,
            new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } },
            cancellationToken);
    }
}
