using System.Net.Http.Json;
using ExteriorServices.Web.Models;

namespace ExteriorServices.Web.Services;

public class CustomerApiClient : ICustomerApiClient
{
    private readonly HttpClient _httpClient;

    public CustomerApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<CustomerSummary>> GetCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<CustomerSummary>>("/api/customers", cancellationToken)
            ?? new List<CustomerSummary>();
    }

    public async Task<CustomerSummary?> GetCustomerAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<CustomerSummary>($"/api/customers/{customerId}", cancellationToken);
    }

    public async Task<CustomerSummary?> CreateCustomerAsync(CustomerCreateRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/customers", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CustomerSummary>(cancellationToken: cancellationToken);
    }

    public async Task<CustomerSummary?> UpdateCustomerAsync(int customerId, CustomerUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/customers/{customerId}", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CustomerSummary>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<PropertySummary>> GetPropertiesForCustomerAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<PropertySummary>>($"/api/customers/{customerId}/properties", cancellationToken)
            ?? new List<PropertySummary>();
    }

    public async Task<PropertySummary?> CreatePropertyAsync(int customerId, PropertyCreateRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/customers/{customerId}/properties", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PropertySummary>(cancellationToken: cancellationToken);
    }

    public async Task<PropertySummary?> UpdatePropertyAsync(int propertyId, PropertyUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/properties/{propertyId}", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PropertySummary>(cancellationToken: cancellationToken);
    }
}
