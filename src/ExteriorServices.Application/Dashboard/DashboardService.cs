namespace ExteriorServices.Application.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var totalCustomers = await _dashboardRepository.GetTotalCustomersAsync(cancellationToken);
        var activeCustomers = await _dashboardRepository.GetActiveCustomersAsync(cancellationToken);
        var totalProperties = await _dashboardRepository.GetTotalPropertiesAsync(cancellationToken);

        return new DashboardDto
        {
            TotalCustomers = totalCustomers,
            ActiveCustomers = activeCustomers,
            TotalProperties = totalProperties
        };
    }
}
