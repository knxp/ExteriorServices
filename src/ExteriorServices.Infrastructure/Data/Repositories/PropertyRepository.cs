using ExteriorServices.Application.Properties;
using ExteriorServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExteriorServices.Infrastructure.Data.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly ExteriorServicesDbContext _dbContext;

    public PropertyRepository(ExteriorServicesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Property>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Property?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Properties.FirstOrDefaultAsync(
            property => property.Id == id,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Property>> GetByCustomerIdAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .AsNoTracking()
            .Where(property => property.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> CustomerExistsAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Customers.AnyAsync(
            customer => customer.Id == customerId,
            cancellationToken);
    }

    public Task AddAsync(
        Property property,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Properties.AddAsync(property, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}