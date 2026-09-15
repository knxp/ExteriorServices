namespace ExteriorServices.Application.Customers;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetCustomersAsync(CancellationToken cancellationToken = default);

    Task<CustomerDto?> GetCustomerAsync(int id, CancellationToken cancellationToken = default);

    Task<CustomerDto> CreateCustomerAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default);

    Task<CustomerDto?> UpdateCustomerAsync(
        int id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeactivateCustomerAsync(int id, CancellationToken cancellationToken = default);
}