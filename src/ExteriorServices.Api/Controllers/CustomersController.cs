using ExteriorServices.Application.Customers;
using ExteriorServices.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ExteriorServices.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetCustomers(
        CancellationToken cancellationToken)
    {
        return Ok(await _customerService.GetCustomersAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetCustomer(
        int id,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetCustomerAsync(id, cancellationToken);

        if (customer is null)
        {
            return NotFound(new ApiError("CustomerNotFound", "The requested customer could not be found."));
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(
        [FromBody] CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.CreateCustomerAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetCustomer),
            new { id = customer.Id },
            customer);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CustomerDto>> UpdateCustomer(
        int id,
        [FromBody] UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.UpdateCustomerAsync(id, request, cancellationToken);

        if (customer is null)
        {
            return NotFound(new ApiError("CustomerNotFound", "The requested customer could not be found."));
        }

        return Ok(customer);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCustomer(
        int id,
        CancellationToken cancellationToken)
    {
        var deactivated = await _customerService.DeactivateCustomerAsync(id, cancellationToken);

        if (!deactivated)
        {
            return NotFound(new ApiError("CustomerNotFound", "The requested customer could not be found."));
        }

        return NoContent();
    }
}