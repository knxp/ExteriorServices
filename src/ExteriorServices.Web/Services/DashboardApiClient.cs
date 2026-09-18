using System.Net.Http.Json;
using ExteriorServices.Web.Models;

namespace ExteriorServices.Web.Services;

public class DashboardApiClient : IDashboardApiClient
{
    private readonly HttpClient _httpClient;

    public DashboardApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DashboardSummary?> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<DashboardSummary>("/api/dashboard", cancellationToken);
    }
}
