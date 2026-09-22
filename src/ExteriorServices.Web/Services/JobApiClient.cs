using System.Net.Http.Json;
using ExteriorServices.Web.Models;

namespace ExteriorServices.Web.Services;

public class JobApiClient : IJobApiClient
{
    private readonly HttpClient _httpClient;

    public JobApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<JobSummary>> GetJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<JobSummary>>("/api/jobs", cancellationToken)
            ?? new List<JobSummary>();
    }

    public async Task<JobSummary?> CreateJobAsync(int customerId, JobCreateRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/customers/{customerId}/jobs", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<JobSummary>(cancellationToken: cancellationToken);
    }
}
