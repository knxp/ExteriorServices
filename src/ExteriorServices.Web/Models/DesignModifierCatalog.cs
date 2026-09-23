namespace ExteriorServices.Web.Models;

/// <summary>
/// Single source of truth for the Generate page's design-option checkboxes: each option's
/// on-screen label and the exact text fed to the AI image prompt (via PromptTemplate/PromptText).
/// Edit here to change what the AI is told without touching the page markup or prompt-building code.
/// </summary>
public static class DesignModifierCatalog
{
    public static IReadOnlyList<ModifierColorOption> Colors { get; } = new[]
    {
        new ModifierColorOption("Warm white", "warm white"),
        new ModifierColorOption("Green", "green"),
        new ModifierColorOption("Red", "red")
    };

    public static IReadOnlyList<ModifierGroup> LightGroups { get; } = new[]
    {
        new ModifierGroup(
            "C9",
            "Classic larger bulbs",
            new[]
            {
                new ModifierOption(
                    "Gutters",
                    "Along the gutters",
                    "{color} C9 Christmas Lights spaced 12 inches apart along EVERY visible horizontal gutter attached to roof perimeter",
                    DefaultChecked: true),
                new ModifierOption(
                    "RoofEdges",
                    "Roof Ridges",
                    "{color} C9 Christmas Lights spaced 12 inches apart along ALL visible roof edge, roof ridge, peak, gable, dormer,"+
                    " and architectural roofline. Light every visible roofline continuously—do not miss any ridges or secondary roof sections.",
                    DefaultChecked: true),
                new ModifierOption(
                    "Windows",
                    "Windows",
                    "{color} C9 (large bulb) mini lights around the complete edges of the windows")
            }),
        new ModifierGroup(
            "MiniLights",
            "Fine, dense light coverage",
            new[]
            {
                new ModifierOption(
                    "Columns",
                    "Columns",
                    "{color} mini string lights wrapped around the house structural vertical columns"),
                new ModifierOption(
                    "Windows",
                    "Windows",
                    "{color} mini string lights around the complete edges of the windows"),
                new ModifierOption(
                    "Bushes",
                    "Bushes",
                    "{color} mini string lights wrapped around visible bushes and shrubs in the photo",
                    DefaultChecked: true),
                new ModifierOption(
                    "Walkways",
                    "Walkways",
                    "{color} mini lights lining the small concrete walkway in the photo. do not include driveways.")
            })
    };

    public static IReadOnlyList<AdditionOption> Additions { get; } = new[]
    {
        new AdditionOption(
            "WreathOnDoor",
            "Wreath on door",
            "a decorative holiday wreath hung on the front door"),
        new AdditionOption(
            "GarlandOnWindows",
            "Garland on windows",
            "christmas holiday garland draped around the windows"),
        new AdditionOption(
            "GarlandOnRailing",
            "Garland on railing",
            "christmas holiday garland wrapped around the visible railing")
    };
}

public sealed record ModifierColorOption(string Label, string PromptText);

/// <param name="PromptTemplate">Use "{color}" as a placeholder for the selected color's PromptText.</param>
public sealed record ModifierOption(
    string Placement,
    string Label,
    string PromptTemplate,
    bool DefaultChecked = false);

public sealed record ModifierGroup(
    string LightType,
    string Description,
    IReadOnlyList<ModifierOption> Options);

public sealed record AdditionOption(
    string Placement,
    string Label,
    string PromptText,
    bool DefaultChecked = false);
