using ExteriorServices.Web.Models;
using ExteriorServices.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExteriorServices.Web.Pages.Admin;

public class CustomersModel : PageModel
{
    private readonly ICustomerApiClient _customerApiClient;

    public CustomersModel(ICustomerApiClient customerApiClient)
    {
        _customerApiClient = customerApiClient;
    }

    [BindProperty]
    public CustomerCreateRequest CustomerForm { get; set; } = new();

    [BindProperty]
    public PropertyCreateRequest PropertyForm { get; set; } = new();

    public IReadOnlyList<CustomerSummary> Customers { get; private set; } = new List<CustomerSummary>();

    public Dictionary<int, List<PropertySummary>> CustomerProperties { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostCustomerAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        await _customerApiClient.CreateCustomerAsync(CustomerForm, cancellationToken);
        TempData["CustomerSaved"] = $"Customer saved successfully: {CustomerForm.FirstName} {CustomerForm.LastName}.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPropertyAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        await _customerApiClient.CreatePropertyAsync(PropertyForm.CustomerId, PropertyForm, cancellationToken);
        TempData["CustomerSaved"] = "Property saved successfully.";
        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Customers = await _customerApiClient.GetCustomersAsync(cancellationToken);

        foreach (var customer in Customers)
        {
            var properties = await _customerApiClient.GetPropertiesForCustomerAsync(customer.Id, cancellationToken);
            CustomerProperties[customer.Id] = properties.ToList();
        }
    }
}
