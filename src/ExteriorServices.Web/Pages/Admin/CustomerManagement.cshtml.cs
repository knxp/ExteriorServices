using ExteriorServices.Web.Models;
using ExteriorServices.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExteriorServices.Web.Pages.Admin;

public class CustomerManagementModel : PageModel
{
    private readonly ICustomerApiClient _customerApiClient;

    public CustomerManagementModel(ICustomerApiClient customerApiClient)
    {
        _customerApiClient = customerApiClient;
    }

    [BindProperty(SupportsGet = true)]
    public string Mode { get; set; } = "add";

    [BindProperty(SupportsGet = true)]
    public int? SelectedCustomerId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? SelectedPropertyId { get; set; }

    [BindProperty]
    public CustomerCreateRequest CustomerForm { get; set; } = new();

    [BindProperty]
    public PropertyUpdateRequest PropertyForm { get; set; } = new();

    public IReadOnlyList<CustomerSummary> Customers { get; private set; } = new List<CustomerSummary>();

    public IReadOnlyList<PropertySummary> CustomerProperties { get; private set; } = new List<PropertySummary>();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);

        if (Mode == "edit" && SelectedCustomerId.HasValue)
        {
            var selectedCustomer = await _customerApiClient.GetCustomerAsync(SelectedCustomerId.Value, cancellationToken);
            if (selectedCustomer is not null)
            {
                CustomerForm = new CustomerCreateRequest
                {
                    FirstName = selectedCustomer.FirstName,
                    LastName = selectedCustomer.LastName,
                    Phone = selectedCustomer.Phone,
                    Email = selectedCustomer.Email
                };
            }

            CustomerProperties = await _customerApiClient.GetPropertiesForCustomerAsync(SelectedCustomerId.Value, cancellationToken);

            if (!SelectedPropertyId.HasValue && CustomerProperties.Count > 0)
            {
                SelectedPropertyId = CustomerProperties.First().Id;
            }

            if (SelectedPropertyId.HasValue)
            {
                var selectedProperty = CustomerProperties.FirstOrDefault(property => property.Id == SelectedPropertyId.Value);
                if (selectedProperty is not null)
                {
                    PropertyForm = new PropertyUpdateRequest
                    {
                        CustomerId = SelectedCustomerId.Value,
                        AddressLine1 = selectedProperty.AddressLine1,
                        AddressLine2 = selectedProperty.AddressLine2,
                        City = selectedProperty.City,
                        State = selectedProperty.State,
                        PostalCode = selectedProperty.PostalCode,
                        PropertyType = selectedProperty.PropertyType,
                        Notes = selectedProperty.Notes
                    };
                }
            }
            else
            {
                PropertyForm = new PropertyUpdateRequest { CustomerId = SelectedCustomerId.Value };
            }
        }
        else if (SelectedCustomerId.HasValue)
        {
            PropertyForm.CustomerId = SelectedCustomerId.Value;
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        if (Mode == "edit" && SelectedCustomerId.HasValue)
        {
            await _customerApiClient.UpdateCustomerAsync(
                SelectedCustomerId.Value,
                new CustomerUpdateRequest
                {
                    FirstName = CustomerForm.FirstName,
                    LastName = CustomerForm.LastName,
                    Phone = CustomerForm.Phone,
                    Email = CustomerForm.Email,
                    IsActive = true
                },
                cancellationToken);

            if (SelectedPropertyId.HasValue)
            {
                await _customerApiClient.UpdatePropertyAsync(
                    SelectedPropertyId.Value,
                    new PropertyUpdateRequest
                    {
                        CustomerId = SelectedCustomerId.Value,
                        AddressLine1 = PropertyForm.AddressLine1,
                        AddressLine2 = PropertyForm.AddressLine2,
                        City = PropertyForm.City,
                        State = PropertyForm.State,
                        PostalCode = PropertyForm.PostalCode,
                        PropertyType = PropertyForm.PropertyType,
                        Notes = PropertyForm.Notes
                    },
                    cancellationToken);

                TempData["CustomerSaved"] = $"Customer and property updated successfully: {CustomerForm.FirstName} {CustomerForm.LastName}.";
            }
            else
            {
                TempData["CustomerSaved"] = $"Customer updated successfully: {CustomerForm.FirstName} {CustomerForm.LastName}.";
            }
        }
        else
        {
            var createdCustomer = await _customerApiClient.CreateCustomerAsync(
                new CustomerCreateRequest
                {
                    FirstName = CustomerForm.FirstName,
                    LastName = CustomerForm.LastName,
                    Phone = CustomerForm.Phone,
                    Email = CustomerForm.Email
                },
                cancellationToken);

            if (createdCustomer is null)
            {
                TempData["CustomerSaved"] = "Customer could not be created.";
                return RedirectToPage("/Admin/Customers");
            }

            var createdProperty = await _customerApiClient.CreatePropertyAsync(
                createdCustomer.Id,
                new PropertyCreateRequest
                {
                    CustomerId = createdCustomer.Id,
                    AddressLine1 = PropertyForm.AddressLine1,
                    AddressLine2 = PropertyForm.AddressLine2,
                    City = PropertyForm.City,
                    State = PropertyForm.State,
                    PostalCode = PropertyForm.PostalCode,
                    PropertyType = PropertyForm.PropertyType,
                    Notes = PropertyForm.Notes
                },
                cancellationToken);

            TempData["CustomerSaved"] = createdProperty is not null
                ? $"Customer and property saved successfully: {CustomerForm.FirstName} {CustomerForm.LastName}."
                : $"Customer saved successfully: {CustomerForm.FirstName} {CustomerForm.LastName}.";
        }

        return RedirectToPage("/Admin/Customers");
    }

    public async Task<IActionResult> OnPostPropertyAsync(CancellationToken cancellationToken)
    {
        var customerId = PropertyForm.CustomerId > 0 ? PropertyForm.CustomerId : SelectedCustomerId ?? 0;

        if (customerId <= 0)
        {
            ModelState.AddModelError(nameof(PropertyForm.CustomerId), "Please select a customer before saving the property.");
            await LoadAsync(cancellationToken);
            return Page();
        }

        PropertyForm.CustomerId = customerId;

        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        if (Mode == "edit" && SelectedPropertyId.HasValue)
        {
            await _customerApiClient.UpdatePropertyAsync(
                SelectedPropertyId.Value,
                new PropertyUpdateRequest
                {
                    CustomerId = customerId,
                    AddressLine1 = PropertyForm.AddressLine1,
                    AddressLine2 = PropertyForm.AddressLine2,
                    City = PropertyForm.City,
                    State = PropertyForm.State,
                    PostalCode = PropertyForm.PostalCode,
                    PropertyType = PropertyForm.PropertyType,
                    Notes = PropertyForm.Notes
                },
                cancellationToken);

            TempData["CustomerSaved"] = "Property updated successfully.";
        }
        else
        {
            await _customerApiClient.CreatePropertyAsync(
                customerId,
                new PropertyCreateRequest
                {
                    CustomerId = customerId,
                    AddressLine1 = PropertyForm.AddressLine1,
                    AddressLine2 = PropertyForm.AddressLine2,
                    City = PropertyForm.City,
                    State = PropertyForm.State,
                    PostalCode = PropertyForm.PostalCode,
                    PropertyType = PropertyForm.PropertyType,
                    Notes = PropertyForm.Notes
                },
                cancellationToken);

            TempData["CustomerSaved"] = "Property saved successfully.";
        }

        return RedirectToPage("/Admin/Customers");
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Customers = await _customerApiClient.GetCustomersAsync(cancellationToken);

        if (SelectedCustomerId.HasValue)
        {
            CustomerProperties = await _customerApiClient.GetPropertiesForCustomerAsync(SelectedCustomerId.Value, cancellationToken);
        }
        else
        {
            CustomerProperties = new List<PropertySummary>();
        }
    }
}
