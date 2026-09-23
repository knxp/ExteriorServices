namespace ExteriorServices.Api.Errors;

public sealed class VisualizationRenderException : Exception
{
    public VisualizationRenderException(string message)
        : base(message)
    {
    }

    public VisualizationRenderException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
