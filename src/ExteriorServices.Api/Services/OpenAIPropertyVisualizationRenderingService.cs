using System.Net.Http.Headers;
using System.Text.Json;
using ExteriorServices.Api.Errors;
using ExteriorServices.Application.Configuration;
using Microsoft.Extensions.Options;

namespace ExteriorServices.Api.Services;

public sealed class OpenAIPropertyVisualizationRenderingService : IPropertyVisualizationRenderingService
{
    public const string HttpClientName = "OpenAI";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OpenAIOptions _options;

    public OpenAIPropertyVisualizationRenderingService(
        IHttpClientFactory httpClientFactory,
        IOptions<OpenAIOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task<VisualizationRenderResult> RenderAsync(
        Stream sourceImage,
        string sourceContentType,
        string prompt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new VisualizationRenderException(
                "The OpenAI API key is not configured. Set OpenAI:ApiKey via user-secrets or environment variables.");
        }

        using var content = new MultipartFormDataContent
        {
            { new StringContent(_options.Model), "model" },
            { new StringContent(prompt), "prompt" },
            { new StringContent(_options.ImageSize), "size" },
            { new StringContent(_options.ImageQuality), "quality" }
        };

        var imageContent = new StreamContent(sourceImage);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue(sourceContentType);
        content.Add(imageContent, "image[]", "source" + ExtensionFor(sourceContentType));

        var client = _httpClientFactory.CreateClient(HttpClientName);
        using var request = new HttpRequestMessage(HttpMethod.Post, "images/edits")
        {
            Content = content
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        using var response = await client.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new VisualizationRenderException(
                $"OpenAI image generation failed with status {(int)response.StatusCode}: {ExtractErrorMessage(responseBody)}");
        }

        var imageBase64 = ExtractImageBase64(responseBody)
            ?? throw new VisualizationRenderException("OpenAI response did not include a generated image.");

        return new VisualizationRenderResult(Convert.FromBase64String(imageBase64), "image/png");
    }

    private static string ExtensionFor(string contentType) => contentType switch
    {
        "image/png" => ".png",
        "image/webp" => ".webp",
        _ => ".jpg"
    };

    private static string? ExtractImageBase64(string responseBody)
    {
        using var document = JsonDocument.Parse(responseBody);
        if (!document.RootElement.TryGetProperty("data", out var data) ||
            data.ValueKind != JsonValueKind.Array ||
            data.GetArrayLength() == 0)
        {
            return null;
        }

        return data[0].TryGetProperty("b64_json", out var b64Json)
            ? b64Json.GetString()
            : null;
    }

    private static string ExtractErrorMessage(string responseBody)
    {
        try
        {
            using var document = JsonDocument.Parse(responseBody);
            if (document.RootElement.TryGetProperty("error", out var error) &&
                error.TryGetProperty("message", out var message))
            {
                return message.GetString() ?? "Unknown error.";
            }
        }
        catch (JsonException)
        {
            // Fall through to the raw body below.
        }

        return responseBody;
    }
}
