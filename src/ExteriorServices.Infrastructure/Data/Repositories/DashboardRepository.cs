using ExteriorServices.Application.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace ExteriorServices.Infrastructure.Data.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly ExteriorServicesDbContext _dbContext;

    public DashboardRepository(ExteriorServicesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> GetTotalCustomersAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Customers
            .AsNoTracking()
            .CountAsync(cancellationToken);
    }

    public Task<int> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Customers
            .AsNoTracking()
            .CountAsync(customer => customer.IsActive, cancellationToken);
    }

    public Task<int> GetTotalPropertiesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Properties
            .AsNoTracking()
            .CountAsync(cancellationToken);
    }
}
