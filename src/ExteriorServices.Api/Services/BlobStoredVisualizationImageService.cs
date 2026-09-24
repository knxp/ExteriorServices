using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ExteriorServices.Api.Models;

namespace ExteriorServices.Api.Services;

// Blob-storage equivalent of StoredVisualizationImageService - same {id}.{ext} /
// {id}-generated.{ext} / {id}.json layout, just backed by Azure Blob Storage instead of disk.
public sealed class BlobStoredVisualizationImageService : IStoredVisualizationImageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStoredVisualizationImageService(BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }

    public IReadOnlyList<StoredVisualizationImage> List()
    {
        var images = new List<StoredVisualizationImage>();

        foreach (var blobItem in _containerClient.GetBlobs())
        {
            if (!blobItem.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var image = ReadMetadata(blobItem.Name);
            if (image is not null)
            {
                images.Add(image);
            }
        }

        return images.OrderByDescending(image => image.CreatedAtUtc).ToList();
    }

    public StoredVisualizationImage? Get(Guid intakeId)
    {
        var blobName = $"{intakeId:N}.json";
        return _containerClient.GetBlobClient(blobName).Exists().Value ? ReadMetadata(blobName) : null;
    }

    public (Stream Stream, string ContentType)? OpenSource(Guid intakeId)
        => OpenFirstMatch($"{intakeId:N}", requiredSuffix: null, excludeSuffixes: new[] { "-generated", "-revised" });

    public (Stream Stream, string ContentType)? OpenResult(Guid intakeId)
        => OpenFirstMatch($"{intakeId:N}-generated", requiredSuffix: "-generated", excludeSuffixes: Array.Empty<string>());

    public (Stream Stream, string ContentType)? OpenRevision(Guid intakeId)
        => OpenFirstMatch($"{intakeId:N}-revised", requiredSuffix: "-revised", excludeSuffixes: Array.Empty<string>());

    private (Stream Stream, string ContentType)? OpenFirstMatch(string prefix, string? requiredSuffix, string[] excludeSuffixes)
    {
        BlobItem? match = null;
        foreach (var blobItem in _containerClient.GetBlobs(traits: BlobTraits.None, states: BlobStates.None, prefix: prefix, cancellationToken: default))
        {
            if (blobItem.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var nameWithoutExtension = Path.GetFileNameWithoutExtension(blobItem.Name);
            if (excludeSuffixes.Any(suffix => nameWithoutExtension.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            if (requiredSuffix is not null && !nameWithoutExtension.EndsWith(requiredSuffix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            match = blobItem;
            break;
        }

        if (match is null)
        {
            return null;
        }

        var blobClient = _containerClient.GetBlobClient(match.Name);
        var download = blobClient.DownloadStreaming();
        var contentType = download.Value.Details.ContentType;
        return (download.Value.Content, string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);
    }

    private StoredVisualizationImage? ReadMetadata(string blobName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            using var stream = blobClient.OpenRead();
            using var metadata = JsonDocument.Parse(stream);
            var root = metadata.RootElement;
            var intakeId = GetProperty(root, "intakeId").GetGuid();
            var contentType = GetProperty(root, "contentType").GetString() ?? "image/jpeg";
            var size = GetProperty(root, "length").GetInt64();
            var createdAtUtc = GetProperty(root, "createdAtUtc").GetDateTime();
            var customerId = ReadNullableInt(root, "CustomerId");
            var propertyId = ReadNullableInt(root, "PropertyId");
            var status = TryGetProperty(root, "status")?.GetString() ?? "Completed";
            var revisedStatus = TryGetProperty(root, "revisedStatus")?.GetString();
            var approved = TryGetProperty(root, "approved")?.GetBoolean() ?? false;

            return new StoredVisualizationImage
            {
                IntakeId = intakeId,
                SourceUrl = $"/api/property-visualizations/{intakeId}/source",
                ResultUrl = status == "Completed" ? $"/api/property-visualizations/{intakeId}/result" : null,
                RevisedResultUrl = revisedStatus == "Completed" ? $"/api/property-visualizations/{intakeId}/revision" : null,
                HasRevision = !string.IsNullOrEmpty(revisedStatus),
                Approved = approved,
                Status = status,
                CustomerId = customerId,
                PropertyId = propertyId,
                ContentType = contentType,
                Size = size,
                CreatedAtUtc = createdAtUtc
            };
        }
        catch (JsonException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private static int? ReadNullableInt(JsonElement root, string propertyName)
    {
        var property = TryGetProperty(root, propertyName);
        return property.HasValue && property.Value.ValueKind != JsonValueKind.Null
            ? property.Value.GetInt32()
            : null;
    }

    private static JsonElement GetProperty(JsonElement root, string propertyName)
    {
        var property = TryGetProperty(root, propertyName);
        return property ?? throw new KeyNotFoundException(propertyName);
    }

    private static JsonElement? TryGetProperty(JsonElement root, string propertyName)
    {
        foreach (var property in root.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return property.Value;
            }
        }

        return null;
    }
}
