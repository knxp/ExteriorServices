namespace ExteriorServices.Application.Configuration;

public sealed class OpenAIOptions
{
    public const string SectionName = "OpenAI";

    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = "chatgpt-image-latest";

    public string ImageSize { get; set; } = "1024x1024";

    public string ImageQuality { get; set; } = "high";
}
