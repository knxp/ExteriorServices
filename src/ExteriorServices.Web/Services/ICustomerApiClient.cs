using ExteriorServices.Web.Models;

namespace ExteriorServices.Web.Services;

public interface ICustomerApiClient
{
    Task<IReadOnlyList<CustomerSummary>> GetCustomersAsync(CancellationToken cancellationToken = default);

    Task<CustomerSummary?> GetCustomerAsync(int customerId, CancellationToken cancellationToken = default);

    Task<CustomerSummary?> CreateCustomerAsync(CustomerCreateRequest request, CancellationToken cancellationToken = default);

    Task<CustomerSummary?> UpdateCustomerAsync(int customerId, CustomerUpdateRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PropertySummary>> GetPropertiesForCustomerAsync(int customerId, CancellationToken cancellationToken = default);

    Task<PropertySummary?> CreatePropertyAsync(int customerId, PropertyCreateRequest request, CancellationToken cancellationToken = default);

    Task<PropertySummary?> UpdatePropertyAsync(int propertyId, PropertyUpdateRequest request, CancellationToken cancellationToken = default);
}
