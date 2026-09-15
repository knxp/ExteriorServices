using ExteriorServices.Application.Properties;
using ExteriorServices.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ExteriorServices.Api.Controllers;

[ApiController]
[Route("api/properties")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertiesController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetProperties(
        CancellationToken cancellationToken)
    {
        return Ok(await _propertyService.GetPropertiesAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
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

    [HttpPut("{id:int}")]
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