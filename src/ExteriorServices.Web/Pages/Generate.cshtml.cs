using ExteriorServices.Web.Models;
using ExteriorServices.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ExteriorServices.Web.Pages;

public class GenerateModel : PageModel
{
    private readonly ICustomerApiClient _customerApiClient;
    private readonly IVisualizationImageApiClient _visualizationImageApiClient;
    private readonly IConfiguration _configuration;

    public GenerateModel(
        ICustomerApiClient customerApiClient,
        IVisualizationImageApiClient visualizationImageApiClient,
        IConfiguration configuration)
    {
        _customerApiClient = customerApiClient;
        _visualizationImageApiClient = visualizationImageApiClient;
        _configuration = configuration;
    }

    [BindProperty(SupportsGet = true)]
    public int? SelectedCustomerId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? SelectedPropertyId { get; set; }

    public IReadOnlyList<CustomerSummary> Customers { get; private set; } = Array.Empty<CustomerSummary>();

    public IReadOnlyList<PropertySummary> Properties { get; private set; } = Array.Empty<PropertySummary>();

    public bool IsAdmin => User.IsInRole("Admin");

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

    // Proxies the intake upload server-side so the OpenAI-backed API never has to be reachable
    // directly from the browser, and so only Admin accounts can trigger a (billable) generation.
    public async Task<IActionResult> OnPostIntakeAsync(CancellationToken cancellationToken)
    {
        if (!IsAdmin)
        {
            return Forbid();
        }

        var form = await Request.ReadFormAsync(cancellationToken);
        var image = form.Files["image"];
        if (image is null || image.Length == 0)
        {
            return BadRequest(new { error = "ValidationError", message = "A property photo is required." });
        }

        using var content = new MultipartFormDataContent();
        var imageContent = new StreamContent(image.OpenReadStream());
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(image.ContentType) ? "application/octet-stream" : image.ContentType);
        content.Add(imageContent, "image", image.FileName);

        foreach (var field in new[] { "customerId", "propertyId", "designOptionsJson", "notes" })
        {
            if (form.TryGetValue(field, out var value) && !string.IsNullOrEmpty(value))
            {
                content.Add(new StringContent(value!), field);
            }
        }

        var apiResponse = await _visualizationImageApiClient.IntakeAsync(content, cancellationToken);
        var body = await apiResponse.Content.ReadAsStringAsync(cancellationToken);
        return new ContentResult
        {
            Content = body,
            ContentType = "application/json",
            StatusCode = (int)apiResponse.StatusCode
        };
    }

    // Proxies the revision request server-side for the same reasons as OnPostIntakeAsync.
    public async Task<IActionResult> OnPostReviseAsync([FromBody] ReviseRequestBody request, CancellationToken cancellationToken)
    {
        if (!IsAdmin)
        {
            return Forbid();
        }

        var apiResponse = await _visualizationImageApiClient.ReviseAsync(request.IntakeId, request.Notes, cancellationToken);
        var body = await apiResponse.Content.ReadAsStringAsync(cancellationToken);
        return new ContentResult
        {
            Content = body,
            ContentType = "application/json",
            StatusCode = (int)apiResponse.StatusCode
        };
    }

    public async Task<IActionResult> OnPostApproveAsync([FromBody] ApproveRequestBody request, CancellationToken cancellationToken)
    {
        if (!IsAdmin)
        {
            return Forbid();
        }

        var apiResponse = await _visualizationImageApiClient.ApproveAsync(request.IntakeId, cancellationToken);
        var body = await apiResponse.Content.ReadAsStringAsync(cancellationToken);
        return new ContentResult
        {
            Content = body,
            ContentType = "application/json",
            StatusCode = (int)apiResponse.StatusCode
        };
    }

    public sealed class ReviseRequestBody
    {
        public Guid IntakeId { get; set; }

        public string Notes { get; set; } = string.Empty;
    }

    public sealed class ApproveRequestBody
    {
        public Guid IntakeId { get; set; }
    }
}

