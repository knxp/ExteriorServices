using ExteriorServices.Api.Models;

namespace ExteriorServices.Api.Services;

public interface IPropertyVisualizationIntakeService
{
    Task<PropertyVisualizationIntakeResponse> IntakeAsync(
        PropertyVisualizationIntakeRequest request,
        CancellationToken cancellationToken = default);
}
