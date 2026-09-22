using ExteriorServices.Web.Models;
using ExteriorServices.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExteriorServices.Web.Pages.Admin;

public class CalendarModel : PageModel
{
    private readonly ICustomerApiClient _customerApiClient;
    private readonly IJobApiClient _jobApiClient;

    public CalendarModel(ICustomerApiClient customerApiClient, IJobApiClient jobApiClient)
    {
        _customerApiClient = customerApiClient;
        _jobApiClient = jobApiClient;
    }

    [BindProperty(SupportsGet = true)]
    public int? SelectedCustomerId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? SelectedPropertyId { get; set; }

    [BindProperty]
    public JobCreateRequest JobForm { get; set; } = new();

    public IReadOnlyList<CustomerSummary> Customers { get; private set; } = Array.Empty<CustomerSummary>();

    public IReadOnlyList<PropertySummary> CustomerProperties { get; private set; } = Array.Empty<PropertySummary>();

    public IReadOnlyList<JobSummary> Jobs { get; private set; } = Array.Empty<JobSummary>();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);

        if (!SelectedCustomerId.HasValue)
        {
            return;
        }

        var customer = await _customerApiClient.GetCustomerAsync(SelectedCustomerId.Value, cancellationToken);
        if (customer is not null)
        {
            JobForm.FirstName = customer.FirstName;
            JobForm.LastName = customer.LastName;
            JobForm.Phone = customer.Phone;
        }

        CustomerProperties = await _customerApiClient.GetPropertiesForCustomerAsync(
            SelectedCustomerId.Value,
            cancellationToken);

        if (!SelectedPropertyId.HasValue)
        {
            return;
        }

        var property = CustomerProperties.FirstOrDefault(p => p.Id == SelectedPropertyId.Value);
        if (property is not null)
        {
            JobForm.AddressLine1 = property.AddressLine1;
            JobForm.City = property.City;
            JobForm.PostalCode = property.PostalCode;
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!SelectedCustomerId.HasValue || SelectedCustomerId <= 0)
        {
            ModelState.AddModelError(string.Empty, "Please select a customer for the job.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        var createdJob = await _jobApiClient.CreateJobAsync(SelectedCustomerId!.Value, JobForm, cancellationToken);

        TempData["JobSaved"] = createdJob is not null
            ? $"Job saved for {createdJob.FirstName} {createdJob.LastName}."
            : "Job could not be created.";

        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Customers = await _customerApiClient.GetCustomersAsync(cancellationToken);
        Jobs = await _jobApiClient.GetJobsAsync(cancellationToken);
    }
}
