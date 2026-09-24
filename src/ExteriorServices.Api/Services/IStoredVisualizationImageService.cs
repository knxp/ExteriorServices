using ExteriorServices.Api.Models;

namespace ExteriorServices.Api.Services;

public interface IStoredVisualizationImageService
{
    IReadOnlyList<StoredVisualizationImage> List();

    StoredVisualizationImage? Get(Guid intakeId);

    (Stream Stream, string ContentType)? OpenSource(Guid intakeId);

    (Stream Stream, string ContentType)? OpenResult(Guid intakeId);

    (Stream Stream, string ContentType)? OpenRevision(Guid intakeId);
}
