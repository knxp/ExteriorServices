using ExteriorServices.Domain.Entities;

namespace ExteriorServices.Application.Jobs;

public interface IJobRepository
{
    Task<IReadOnlyList<Job>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Job?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Job>> GetByCustomerIdAsync(
        int customerId,
        CancellationToken cancellationToken = default);

    Task<bool> CustomerExistsAsync(int customerId, CancellationToken cancellationToken = default);

    Task AddAsync(Job job, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
