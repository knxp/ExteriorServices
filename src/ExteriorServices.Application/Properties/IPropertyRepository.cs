using ExteriorServices.Domain.Entities;

namespace ExteriorServices.Application.Properties;

public interface IPropertyRepository
{
    Task<IReadOnlyList<Property>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Property>> GetByCustomerIdAsync(
        int customerId,
        CancellationToken cancellationToken = default);

    Task<bool> CustomerExistsAsync(int customerId, CancellationToken cancellationToken = default);

    Task AddAsync(Property property, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}