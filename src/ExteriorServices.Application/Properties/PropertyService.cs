using ExteriorServices.Domain.Entities;

namespace ExteriorServices.Application.Properties;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<IReadOnlyList<PropertyDto>> GetPropertiesAsync(
        CancellationToken cancellationToken = default)
    {
        var properties = await _propertyRepository.GetAllAsync(cancellationToken);
        return properties.Select(Map).ToList();
    }

    public async Task<PropertyDto?> GetPropertyAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(id, cancellationToken);
        return property is null ? null : Map(property);
    }

    public async Task<IReadOnlyList<PropertyDto>> GetCustomerPropertiesAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        var properties = await _propertyRepository.GetByCustomerIdAsync(
            customerId,
            cancellationToken);
        return properties.Select(Map).ToList();
    }

    public async Task<PropertyDto?> CreatePropertyAsync(
        int customerId,
        CreatePropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await _propertyRepository.CustomerExistsAsync(customerId, cancellationToken))
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var property = new Property
        {
            CustomerId = customerId,
            AddressLine1 = request.AddressLine1.Trim(),
            AddressLine2 = request.AddressLine2,
            City = request.City.Trim(),
            State = request.State.Trim(),
            PostalCode = request.PostalCode.Trim(),
            PropertyType = request.PropertyType,
            Notes = request.Notes,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _propertyRepository.AddAsync(property, cancellationToken);
        await _propertyRepository.SaveChangesAsync(cancellationToken);

        return Map(property);
    }

    public async Task<PropertyDto?> UpdatePropertyAsync(
        int id,
        UpdatePropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(id, cancellationToken);
        if (property is null)
        {
            return null;
        }

        property.AddressLine1 = request.AddressLine1.Trim();
        property.AddressLine2 = request.AddressLine2;
        property.City = request.City.Trim();
        property.State = request.State.Trim();
        property.PostalCode = request.PostalCode.Trim();
        property.PropertyType = request.PropertyType;
        property.Notes = request.Notes;
        property.UpdatedAt = DateTime.UtcNow;

        await _propertyRepository.SaveChangesAsync(cancellationToken);
        return Map(property);
    }

    private static PropertyDto Map(Property property) => new()
    {
        Id = property.Id,
        CustomerId = property.CustomerId,
        AddressLine1 = property.AddressLine1,
        AddressLine2 = property.AddressLine2,
        City = property.City,
        State = property.State,
        PostalCode = property.PostalCode,
        PropertyType = property.PropertyType,
        Notes = property.Notes,
        CreatedAt = property.CreatedAt,
        UpdatedAt = property.UpdatedAt
    };
}
