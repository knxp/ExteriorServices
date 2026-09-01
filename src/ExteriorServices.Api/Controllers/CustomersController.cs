using ExteriorServices.Domain.Entities;
using ExteriorServices.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExteriorServices.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ExteriorServicesDbContext _db;

    public CustomersController(ExteriorServicesDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Customer>>> GetCustomers()
    {
        var customers = await _db.Customers
            .Include(x => x.Properties)
            .AsNoTracking()
            .ToListAsync();

        return Ok(customers);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var customer = await _db.Customers
            .Include(x => x.Properties)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer([FromBody] Customer customer)
    {
        customer.Id = 0;
        customer.CreatedAt = DateTime.UtcNow;
        customer.UpdatedAt = DateTime.UtcNow;

        foreach (var property in customer.Properties)
        {
            property.CustomerId = 0;
            property.CreatedAt = DateTime.UtcNow;
            property.UpdatedAt = DateTime.UtcNow;
            property.Customer = null;
        }

        _db.Customers.Add(customer);

        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCustomer),
            new { id = customer.Id },
            customer);
    }
}