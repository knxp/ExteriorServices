using ExteriorServices.Web.Models;

namespace ExteriorServices.Web.Services;

public interface IDashboardApiClient
{
    Task<DashboardSummary?> GetDashboardAsync(CancellationToken cancellationToken = default);
}
