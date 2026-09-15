using ExteriorServices.Application.Customers;
using ExteriorServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExteriorServices.Infrastructure.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly ExteriorServicesDbContext _dbContext;

    public CustomerRepository(ExteriorServicesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Customer?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Customers.FirstOrDefaultAsync(
            customer => customer.Id == id,
            cancellationToken);
    }

    public Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Customers.AddAsync(customer, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}