using System.Text;
using System.Text.Json;

namespace ExteriorServices.Api.Services;

public static class VisualizationPromptBuilder
{
    public static string Build(string? designOptionsJson, string? notes)
    {
        var builder = new StringBuilder(
            "Photorealistic nighttime edit of the provided house photo. Keep the exact same house, architecture, camera angle, perspective," +
            " landscaping, and composition. Do not add, remove, or reshape any structural elements. Lights must follow the exact " +
            "shape of the house and look professionally attached with realistic clips." +
            " Make it nighttime with subtle stars and a winter atmosphere, no snow. Darken" +
            " the surroundings so the lights stand out. Make everything look photorealistic and professionally installed, with realistic glow" +
            " and reflections."
        );

        /*
        "Photorealistic edit of the provided house photograph. It must be dark outside, subtle stars in the sky with a winter ambiance NO SNOW. " +
                    "Keep the exact same house, architecture, camera angle, perspective, composition, landscaping, siding, windows, doors, roof shape, and original photo." +
                    "Do not add, remove, or reshape any structural elements." +
                    "Add only the following holiday decorations, installed so they look professionally placed and physically attached to the real house." +
                    "Make sure its dark outside and the lights should pop:"
        */

        var placements = ParsePlacements(designOptionsJson);
        if (placements.Count > 0)
        {
            builder.Append(" Add: ");
            builder.Append(string.Join("; ", placements));
            builder.Append('.');
        }

        if (!string.IsNullOrWhiteSpace(notes))
        {
            builder.Append(" Additional instructions: ");
            builder.Append(notes.Trim());
        }

        return builder.ToString();
    }

    public static string BuildRevision(string notes)
    {
        return "Take this edited image and make the following minor adjustments: " + notes.Trim();
    }

    private static List<string> ParsePlacements(string? designOptionsJson)
    {
        var placements = new List<string>();
        if (string.IsNullOrWhiteSpace(designOptionsJson))
        {
            return placements;
        }

        try
        {
            using var document = JsonDocument.Parse(designOptionsJson);
            if (!document.RootElement.TryGetProperty("modifiers", out var modifiers) ||
                modifiers.ValueKind != JsonValueKind.Array)
            {
                return placements;
            }

            foreach (var modifier in modifiers.EnumerateArray())
            {
                var explicitPrompt = modifier.TryGetProperty("prompt", out var promptValue)
                    ? promptValue.GetString()
                    : null;

                if (!string.IsNullOrWhiteSpace(explicitPrompt))
                {
                    placements.Add(explicitPrompt.Trim());
                    continue;
                }

                var placement = modifier.TryGetProperty("placement", out var placementValue)
                    ? placementValue.GetString()
                    : null;

                if (string.IsNullOrWhiteSpace(placement))
                {
                    continue;
                }

                var lightType = modifier.TryGetProperty("lightType", out var lightTypeValue)
                    ? lightTypeValue.GetString()
                    : null;
                var color = modifier.TryGetProperty("color", out var colorValue)
                    ? colorValue.GetString()
                    : null;

                var description = lightType is null
                    ? placement
                    : $"{lightType} lights on the {placement} spaced 12 inches";

                if (!string.IsNullOrWhiteSpace(color))
                {
                    description += $" in {color}";
                }

                placements.Add(description);
            }
        }
        catch (JsonException)
        {
            // Malformed design options are ignored; the base prompt is still usable.
        }

        return placements;
    }
}
