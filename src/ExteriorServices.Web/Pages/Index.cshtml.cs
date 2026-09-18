using ExteriorServices.Web.Models;
using ExteriorServices.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    private readonly IDashboardApiClient _dashboardApiClient;

    public IndexModel(IDashboardApiClient dashboardApiClient)
    {
        _dashboardApiClient = dashboardApiClient;
    }

    public DashboardSummary? Dashboard { get; private set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Dashboard = await _dashboardApiClient.GetDashboardAsync(cancellationToken);
    }
}
