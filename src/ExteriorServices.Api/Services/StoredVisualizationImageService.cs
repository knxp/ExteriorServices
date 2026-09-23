using System.Text.Json;
using ExteriorServices.Api.Models;

namespace ExteriorServices.Api.Services;

public sealed class StoredVisualizationImageService : IStoredVisualizationImageService
{
    private readonly string _storageDirectory;

    public StoredVisualizationImageService(IWebHostEnvironment environment)
    {
        _storageDirectory = Path.Combine(environment.ContentRootPath, "App_Data", "visualizations");
    }

    public IReadOnlyList<StoredVisualizationImage> List()
    {
        if (!Directory.Exists(_storageDirectory))
        {
            return Array.Empty<StoredVisualizationImage>();
        }

        return Directory.EnumerateFiles(_storageDirectory, "*.json")
            .Select(ReadMetadata)
            .Where(image => image is not null)
            .Select(image => image!)
            .OrderByDescending(image => image.CreatedAtUtc)
            .ToList();
    }

    public (Stream Stream, string ContentType)? OpenSource(Guid intakeId)
    {
        var path = FindImagePath(intakeId);
        if (path is null)
        {
            return null;
        }

        var contentType = Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };

        return (File.OpenRead(path), contentType);
    }

    public (Stream Stream, string ContentType)? OpenResult(Guid intakeId)
    {
        var path = FindResultPath(intakeId);
        if (path is null)
        {
            return null;
        }

        var contentType = Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".webp" => "image/webp",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "image/png"
        };

        return (File.OpenRead(path), contentType);
    }

    private StoredVisualizationImage? ReadMetadata(string metadataPath)
    {
        try
        {
            using var stream = File.OpenRead(metadataPath);
            using var metadata = JsonDocument.Parse(stream);
            var root = metadata.RootElement;
            var intakeId = GetProperty(root, "intakeId").GetGuid();
            var contentType = GetProperty(root, "contentType").GetString() ?? "image/jpeg";
            var size = GetProperty(root, "length").GetInt64();
            var createdAtUtc = GetProperty(root, "createdAtUtc").GetDateTime();
            var customerId = ReadNullableInt(root, "CustomerId");
            var propertyId = ReadNullableInt(root, "PropertyId");
            var status = TryGetProperty(root, "status")?.GetString() ?? "Completed";

            return new StoredVisualizationImage
            {
                IntakeId = intakeId,
                SourceUrl = $"/api/property-visualizations/{intakeId}/source",
                ResultUrl = status == "Completed" ? $"/api/property-visualizations/{intakeId}/result" : null,
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

    private string? FindImagePath(Guid intakeId)
    {
        if (!Directory.Exists(_storageDirectory))
        {
            return null;
        }

        return Directory.EnumerateFiles(_storageDirectory, $"{intakeId:N}.*")
            .FirstOrDefault(path => !path.EndsWith(".json", StringComparison.OrdinalIgnoreCase));
    }

    private string? FindResultPath(Guid intakeId)
    {
        if (!Directory.Exists(_storageDirectory))
        {
            return null;
        }

        return Directory.EnumerateFiles(_storageDirectory, $"{intakeId:N}-generated.*")
            .FirstOrDefault();
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
