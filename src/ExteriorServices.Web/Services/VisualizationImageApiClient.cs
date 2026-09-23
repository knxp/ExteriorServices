using System.Net.Http.Json;
using ExteriorServices.Web.Models;

namespace ExteriorServices.Web.Services;

public sealed class VisualizationImageApiClient : IVisualizationImageApiClient
{
    private readonly HttpClient _httpClient;

    public VisualizationImageApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<VisualizationImageSummary>> GetImagesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<IReadOnlyList<VisualizationImageSummary>>(
                   "/api/property-visualizations",
                   cancellationToken)
               ?? Array.Empty<VisualizationImageSummary>();
    }

    public Task<HttpResponseMessage> IntakeAsync(MultipartFormDataContent content, CancellationToken cancellationToken = default)
    {
        return _httpClient.PostAsync("/api/property-visualizations/intake", content, cancellationToken);
    }
}
