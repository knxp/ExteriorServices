namespace ExteriorServices.Application.Customers;

using ExteriorServices.Domain.Entities;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IReadOnlyList<CustomerDto>> GetCustomersAsync(
        CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        return customers.Select(Map).ToList();
    }

    public async Task<CustomerDto?> GetCustomerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        return customer is null ? null : Map(customer);
    }

    public async Task<CustomerDto> CreateCustomerAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var customer = new Customer
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Phone = request.Phone,
            Email = request.Email,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _customerRepository.SaveChangesAsync(cancellationToken);

        return Map(customer);
    }

    public async Task<CustomerDto?> UpdateCustomerAsync(
        int id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return null;
        }

        customer.FirstName = request.FirstName.Trim();
        customer.LastName = request.LastName.Trim();
        customer.Phone = request.Phone;
        customer.Email = request.Email;
        customer.IsActive = request.IsActive;
        customer.UpdatedAt = DateTime.UtcNow;

        await _customerRepository.SaveChangesAsync(cancellationToken);
        return Map(customer);
    }

    public async Task<bool> DeactivateCustomerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return false;
        }

        if (customer.IsActive)
        {
            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;
            await _customerRepository.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    private static CustomerDto Map(Customer customer) => new()
    {
        Id = customer.Id,
        FirstName = customer.FirstName,
        LastName = customer.LastName,
        Phone = customer.Phone,
        Email = customer.Email,
        IsActive = customer.IsActive,
        CreatedAt = customer.CreatedAt,
        UpdatedAt = customer.UpdatedAt
    };
}
