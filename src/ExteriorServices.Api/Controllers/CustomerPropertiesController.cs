using ExteriorServices.Application.Properties;
using ExteriorServices.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ExteriorServices.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:int}/properties")]
public class CustomerPropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public CustomerPropertiesController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetProperties(
        int customerId,
        CancellationToken cancellationToken)
    {
        return Ok(await _propertyService.GetCustomerPropertiesAsync(customerId, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<PropertyDto>> CreateProperty(
        int customerId,
        [FromBody] CreatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        var property = await _propertyService.CreatePropertyAsync(
            customerId,
            request,
            cancellationToken);

        if (property is null)
        {
            return BadRequest(new ApiError(
                "CustomerNotFound",
                "The requested customer could not be found."));
        }

        return CreatedAtAction(
            nameof(PropertiesController.GetProperty),
            "Properties",
            new { id = property.Id },
            property);
    }
}