namespace ExteriorServices.Api.Services;

public sealed record VisualizationRenderResult(byte[] ImageBytes, string ContentType);

public interface IPropertyVisualizationRenderingService
{
    Task<VisualizationRenderResult> RenderAsync(
        Stream sourceImage,
        string sourceContentType,
        string prompt,
        CancellationToken cancellationToken = default);
}
