using ExteriorServices.Api.Models;

namespace ExteriorServices.Api.Services;

public interface IPropertyVisualizationRevisionService
{
    Task<StoredVisualizationImage> ReviseAsync(Guid intakeId, string notes, CancellationToken cancellationToken = default);

    Task<StoredVisualizationImage> ApproveAsync(Guid intakeId, CancellationToken cancellationToken = default);
}
