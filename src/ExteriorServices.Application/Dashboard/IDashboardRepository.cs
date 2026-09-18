namespace ExteriorServices.Application.Dashboard;

public interface IDashboardRepository
{
    Task<int> GetTotalCustomersAsync(CancellationToken cancellationToken = default);

    Task<int> GetActiveCustomersAsync(CancellationToken cancellationToken = default);

    Task<int> GetTotalPropertiesAsync(CancellationToken cancellationToken = default);
}
