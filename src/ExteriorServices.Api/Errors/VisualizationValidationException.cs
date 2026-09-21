namespace ExteriorServices.Api.Errors;

public sealed class VisualizationValidationException : Exception
{
    public VisualizationValidationException(string message)
        : base(message)
    {
    }
}
