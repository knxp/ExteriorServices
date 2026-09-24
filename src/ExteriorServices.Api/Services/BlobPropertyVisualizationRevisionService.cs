using System.Text;
using System.Text.Json.Nodes;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ExteriorServices.Api.Errors;
using ExteriorServices.Api.Models;

namespace ExteriorServices.Api.Services;

// Blob-storage equivalent of LocalPropertyVisualizationRevisionService - edits the same
// {id}.json metadata blob and writes a {id}-revised.{ext} blob.
public sealed class BlobPropertyVisualizationRevisionService : IPropertyVisualizationRevisionService
{
    private readonly BlobContainerClient _containerClient;
    private readonly IPropertyVisualizationRenderingService _renderingService;
    private readonly IStoredVisualizationImageService _imageService;
    private readonly ILogger<BlobPropertyVisualizationRevisionService> _logger;

    public BlobPropertyVisualizationRevisionService(
        BlobContainerClient containerClient,
        IPropertyVisualizationRenderingService renderingService,
        IStoredVisualizationImageService imageService,
        ILogger<BlobPropertyVisualizationRevisionService> logger)
    {
        _containerClient = containerClient;
        _renderingService = renderingService;
        _imageService = imageService;
        _logger = logger;
    }

    public async Task<StoredVisualizationImage> ReviseAsync(
        Guid intakeId,
        string notes,
        CancellationToken cancellationToken = default)
    {
        var existing = _imageService.Get(intakeId)
            ?? throw new VisualizationValidationException("The requested visualization could not be found.");

        if (existing.Status != "Completed")
        {
            throw new VisualizationValidationException("Only a successfully generated preview can be revised.");
        }

        if (existing.HasRevision)
        {
            throw new VisualizationValidationException("This image has already been revised. Only one revision is allowed.");
        }

        var metadataBlobName = $"{intakeId:N}.json";
        var metadataNode = await ReadMetadataAsync(metadataBlobName, cancellationToken);

        var prompt = VisualizationPromptBuilder.BuildRevision(notes);
        var generatedSource = _imageService.OpenResult(intakeId)
            ?? throw new VisualizationValidationException("The generated preview image could not be found.");

        var revisedStatus = "Completed";
        string? revisedErrorMessage = null;

        try
        {
            using var stream = generatedSource.Stream;
            var renderResult = await _renderingService.RenderAsync(
                stream,
                generatedSource.ContentType,
                prompt,
                cancellationToken);

            var extension = renderResult.ContentType switch
            {
                "image/webp" => ".webp",
                "image/jpeg" => ".jpg",
                _ => ".png"
            };

            var revisedBlobName = $"{intakeId:N}-revised{extension}";
            using var revisedStream = new MemoryStream(renderResult.ImageBytes);
            await UploadAsync(revisedBlobName, revisedStream, renderResult.ContentType, cancellationToken);
        }
        catch (VisualizationRenderException exception)
        {
            _logger.LogError(exception, "Failed to render revision for intake {IntakeId}.", intakeId);
            revisedStatus = "Failed";
            revisedErrorMessage = exception.Message;
        }

        metadataNode["revisionNotes"] = notes;
        metadataNode["revisedStatus"] = revisedStatus;
        metadataNode["revisedErrorMessage"] = revisedErrorMessage;

        await WriteMetadataAsync(metadataBlobName, metadataNode, cancellationToken);

        return _imageService.Get(intakeId)!;
    }

    public async Task<StoredVisualizationImage> ApproveAsync(
        Guid intakeId,
        CancellationToken cancellationToken = default)
    {
        var existing = _imageService.Get(intakeId)
            ?? throw new VisualizationValidationException("The requested visualization could not be found.");

        if (existing.Status != "Completed")
        {
            throw new VisualizationValidationException("There is no generated preview to approve yet.");
        }

        var metadataBlobName = $"{intakeId:N}.json";
        var metadataNode = await ReadMetadataAsync(metadataBlobName, cancellationToken);

        metadataNode["approved"] = true;
        metadataNode["approvedAtUtc"] = DateTime.UtcNow;

        await WriteMetadataAsync(metadataBlobName, metadataNode, cancellationToken);

        return _imageService.Get(intakeId)!;
    }

    private async Task<JsonObject> ReadMetadataAsync(string blobName, CancellationToken cancellationToken)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        var download = await blobClient.DownloadContentAsync(cancellationToken);
        return JsonNode.Parse(download.Value.Content.ToString())!.AsObject();
    }

    private async Task WriteMetadataAsync(string blobName, JsonObject metadataNode, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(metadataNode.ToJsonString()));
        await UploadAsync(blobName, stream, "application/json", cancellationToken);
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
