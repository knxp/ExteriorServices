using System.Text.Json.Nodes;
using ExteriorServices.Api.Errors;
using ExteriorServices.Api.Models;

namespace ExteriorServices.Api.Services;

// Local-disk equivalent of BlobPropertyVisualizationRevisionService - reads/edits the same
// {id}.json metadata used by LocalPropertyVisualizationIntakeService and writes {id}-revised.{ext}.
public sealed class LocalPropertyVisualizationRevisionService : IPropertyVisualizationRevisionService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IPropertyVisualizationRenderingService _renderingService;
    private readonly IStoredVisualizationImageService _imageService;
    private readonly ILogger<LocalPropertyVisualizationRevisionService> _logger;

    public LocalPropertyVisualizationRevisionService(
        IWebHostEnvironment environment,
        IPropertyVisualizationRenderingService renderingService,
        IStoredVisualizationImageService imageService,
        ILogger<LocalPropertyVisualizationRevisionService> logger)
    {
        _environment = environment;
        _renderingService = renderingService;
        _imageService = imageService;
        _logger = logger;
    }

    private string StorageDirectory => Path.Combine(_environment.ContentRootPath, "App_Data", "visualizations");

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

        var metadataPath = Path.Combine(StorageDirectory, $"{intakeId:N}.json");
        var metadataNode = JsonNode.Parse(await File.ReadAllTextAsync(metadataPath, cancellationToken))!.AsObject();

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

            var revisedPath = Path.Combine(StorageDirectory, $"{intakeId:N}-revised{extension}");
            await File.WriteAllBytesAsync(revisedPath, renderResult.ImageBytes, cancellationToken);
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

        await File.WriteAllTextAsync(metadataPath, metadataNode.ToJsonString(), cancellationToken);

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

        var metadataPath = Path.Combine(StorageDirectory, $"{intakeId:N}.json");
        var metadataNode = JsonNode.Parse(await File.ReadAllTextAsync(metadataPath, cancellationToken))!.AsObject();

        metadataNode["approved"] = true;
        metadataNode["approvedAtUtc"] = DateTime.UtcNow;

        await File.WriteAllTextAsync(metadataPath, metadataNode.ToJsonString(), cancellationToken);

        return _imageService.Get(intakeId)!;
    }
}
