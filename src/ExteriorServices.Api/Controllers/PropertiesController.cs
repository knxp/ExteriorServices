using ExteriorServices.Application.Properties;
using ExteriorServices.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ExteriorServices.Api.Controllers;

[ApiController]
[Route("api")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertiesController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpGet("properties")]
    public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetProperties(
        CancellationToken cancellationToken)
    {
        return Ok(await _propertyService.GetPropertiesAsync(cancellationToken));
    }

    [HttpGet("properties/{id:int}")]
    public async Task<ActionResult<PropertyDto>> GetProperty(
        int id,
        CancellationToken cancellationToken)
    {
        var property = await _propertyService.GetPropertyAsync(id, cancellationToken);

        if (property is null)
        {
            return NotFound(new ApiError("PropertyNotFound", "The requested property could not be found."));
        }

        return Ok(property);
    }

    [HttpGet("customers/{customerId:int}/properties")]
    public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetCustomerProperties(
        int customerId,
        CancellationToken cancellationToken)
    {
        return Ok(await _propertyService.GetCustomerPropertiesAsync(customerId, cancellationToken));
    }

    [HttpPost("customers/{customerId:int}/properties")]
    public async Task<ActionResult<PropertyDto>> CreatePropertyForCustomer(
        int customerId,
        [FromBody] CreatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        var property = await _propertyService.CreatePropertyAsync(customerId, request, cancellationToken);

        if (property is null)
        {
            return NotFound(new ApiError("CustomerNotFound", "The requested customer could not be found."));
        }

        return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, property);
    }

    [HttpPut("properties/{id:int}")]
    public async Task<ActionResult<PropertyDto>> UpdateProperty(
        int id,
        [FromBody] UpdatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        var property = await _propertyService.UpdatePropertyAsync(id, request, cancellationToken);

        if (property is null)
        {
            return NotFound(new ApiError("PropertyNotFound", "The requested property could not be found."));
        }

        return Ok(property);
    }
}