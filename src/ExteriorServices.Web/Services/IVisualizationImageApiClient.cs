using ExteriorServices.Web.Models;

namespace ExteriorServices.Web.Services;

public interface IVisualizationImageApiClient
{
    Task<IReadOnlyList<VisualizationImageSummary>> GetImagesAsync(CancellationToken cancellationToken = default);

    Task<HttpResponseMessage> IntakeAsync(MultipartFormDataContent content, CancellationToken cancellationToken = default);
}
