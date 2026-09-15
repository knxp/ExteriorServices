namespace ExteriorServices.Application.Properties;

public interface IPropertyService
{
    Task<IReadOnlyList<PropertyDto>> GetPropertiesAsync(
        CancellationToken cancellationToken = default);

    Task<PropertyDto?> GetPropertyAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PropertyDto>> GetCustomerPropertiesAsync(
        int customerId,
        CancellationToken cancellationToken = default);

    Task<PropertyDto?> CreatePropertyAsync(
        int customerId,
        CreatePropertyRequest request,
        CancellationToken cancellationToken = default);

    Task<PropertyDto?> UpdatePropertyAsync(
        int id,
        UpdatePropertyRequest request,
        CancellationToken cancellationToken = default);
}