using ExteriorServices.Domain.Entities;
using ExteriorServices.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExteriorServices.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly ExteriorServicesDbContext _db;

    public PropertiesController(ExteriorServicesDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Property>>> GetProperties()
    {
        var properties = await _db.Properties
            .AsNoTracking()
            .ToListAsync();

        return Ok(properties);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Property>> GetProperty(int id)
    {
        var property = await _db.Properties
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (property is null)
        {
            return NotFound();
        }

        return Ok(property);
    }

    [HttpPost]
    public async Task<ActionResult<Property>> CreateProperty(Property property)
    {
        var customerExists = await _db.Customers
            .AnyAsync(x => x.Id == property.CustomerId);

        if (!customerExists)
        {
            return BadRequest("Customer does not exist.");
        }

        property.Id = 0;
        property.CreatedAt = DateTime.UtcNow;
        property.UpdatedAt = DateTime.UtcNow;

        _db.Properties.Add(property);

        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProperty),
            new { id = property.Id },
            property);
    }
}