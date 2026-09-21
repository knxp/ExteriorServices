using ExteriorServices.Web.Models;
using ExteriorServices.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExteriorServices.Web.Pages;

public class GenerateModel : PageModel
{
    private readonly ICustomerApiClient _customerApiClient;
    private readonly IConfiguration _configuration;

    public GenerateModel(
        ICustomerApiClient customerApiClient,
        IConfiguration configuration)
    {
        _customerApiClient = customerApiClient;
        _configuration = configuration;
    }

    [BindProperty(SupportsGet = true)]
    public int? SelectedCustomerId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? SelectedPropertyId { get; set; }

    public IReadOnlyList<CustomerSummary> Customers { get; private set; } = Array.Empty<CustomerSummary>();

    public IReadOnlyList<PropertySummary> Properties { get; private set; } = Array.Empty<PropertySummary>();

    public string BrowserApiBaseUrl => _configuration["BrowserApiBaseUrl"]
        ?? _configuration["ApiBaseUrl"]
        ?? "https://localhost:5001";

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Customers = await _customerApiClient.GetCustomersAsync(cancellationToken);

        if (SelectedCustomerId.HasValue)
        {
            Properties = await _customerApiClient.GetPropertiesForCustomerAsync(
                SelectedCustomerId.Value,
                cancellationToken);
        }
    }
}
