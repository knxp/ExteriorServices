using ExteriorServices.Application.Jobs;
using ExteriorServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExteriorServices.Infrastructure.Data.Repositories;

public class JobRepository : IJobRepository
{
    private readonly ExteriorServicesDbContext _dbContext;

    public JobRepository(ExteriorServicesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Job>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Jobs
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Job?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Jobs.FirstOrDefaultAsync(
            job => job.Id == id,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Job>> GetByCustomerIdAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Jobs
            .AsNoTracking()
            .Where(job => job.CustomerId == customerId)
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
        Job job,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Jobs.AddAsync(job, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
