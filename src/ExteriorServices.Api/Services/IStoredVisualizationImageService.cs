using ExteriorServices.Api.Models;

namespace ExteriorServices.Api.Services;

public interface IStoredVisualizationImageService
{
    IReadOnlyList<StoredVisualizationImage> List();

    (Stream Stream, string ContentType)? OpenSource(Guid intakeId);
}
