using System.Collections.Generic;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PatientPortal.TagHelpers.Buttons;

/// <summary>
/// Defines the visual button style variants.
/// </summary>
public enum ButtonVariant
{
    /// <summary>
    /// The primary variant, typically used for main actions: form submit, go to, or similar.
    /// </summary>
    Primary,

    /// <summary>
    /// The secondary variant, typically used for less prominent actions: cancel submit, go back, or similar.
    /// </summary>
    Secondary,

    /// <summary>
    /// The warning variant, typically used for cautionary actions.
    /// </summary>
    Warning,

    /// <summary>
    /// The danger variant, typically used for destructive actions.
    /// </summary>
    Danger,
}

public enum HoverEffect
{
    Lighten,
    Darken,
    None,
}

/// <summary>
/// Interface for a button style tag helper, defining properties for button styling.
/// </summary>
public interface IButtonStyleable
{
    /// <summary>
    /// The button visual style. "primary", "secondary", "warning" or "danger".
    /// </summary>
    ButtonVariant Variant { get; }

    /// <summary>
    /// If true, applies the <c>btn-sm</c> Bootstrap size modifier.
    /// </summary>
    bool IsSmall { get; set; }

    /// <summary>
    /// If true, uses the outline variant for the button style.
    /// For "primary" and "secondary", applies a custom outline class; for others, uses Bootstrap outline classes.
    /// </summary>
    bool Outline { get; set; }

    /// <summary>
    /// The hover effect to apply to the button. "lighten", "darken", or "none" (default).
    /// </summary>
    HoverEffect Hover { get; set; }
}

/// <summary>
/// Extension methods for applying button style classes based on the
/// IButtonStyleable properties.
/// </summary>
public static class ButtonStyleExtensions
{
    /// <summary>
    /// Gets the appropriate hover modifier class based on the Hover property of
    /// the IButtonStyleable element.
    /// </summary>
    /// <param name="element">The element implementing IButtonStyleable.</param>
    /// <returns>The CSS class for the hover effect.</returns>
    private static string GetHoverModifierClass(this IButtonStyleable element)
    {
        return element.Hover switch
        {
            HoverEffect.Lighten => "hover-lighten",
            HoverEffect.Darken => "hover-darken",
            _ => string.Empty,
        };
    }

    /// <summary>
    /// Gets the appropriate custom hue classes based on the color parameter and
    /// the Outline and Hover properties of the IButtonStyleable element.
    /// </summary>
    /// <param name="element">The element implementing IButtonStyleable.</param>
    /// <param name="color">The color variant of the button.</param>
    /// <returns>A list of CSS classes for the custom hue.</returns>
    private static List<string> GetCustomHueClass(this IButtonStyleable element, string color)
    {
        List<string> hueClasses = color switch
        {
            "primary" => ["hue-primary", "and-dimmed"],
            "secondary" => ["hue-secondary"],
            "warning" => ["hue-warning"],
            _ => [],
        };

        List<string> outlineClass = element.Outline
            ? ["on-btn-outline"]
            : [element.GetHoverModifierClass(), "on-btn"];
        hueClasses.AddRange(outlineClass);

        return hueClasses;
    }

    /// <summary>
    /// Gets the appropriate Bootstrap variant class based on the color parameter
    /// and the Outline property of the IButtonStyleable element. Used for
    /// "danger" variant.
    /// </summary>
    /// <param name="element">The element implementing IButtonStyleable.</param>
    /// <param name="color">The color variant of the button.</param>
    /// <returns>A list of CSS classes for the Bootstrap variant.</returns>
    private static List<string> GetBootstrapVariantClass(
        this IButtonStyleable element,
        string color
    )
    {
        return element.Outline ? [$"btn-outline-{color}"] : [$"btn-{color}"];
    }

    /// <summary>
    /// Builds the list of CSS classes to apply to the button based on the
    /// properties of the IButtonStyleable element.
    /// </summary>
    /// <param name="element">The element implementing IButtonStyleable.</param>
    /// <returns>A list of CSS classes for the button.</returns>
    private static List<string> BuildClasses(this IButtonStyleable element)
    {
        List<string> buttonStyleClasses = ["btn"];

        if (element.IsSmall)
        {
            buttonStyleClasses.Add("btn-sm");
        }

        buttonStyleClasses.AddRange(
            element.Variant switch
            {
                ButtonVariant.Primary => element.GetCustomHueClass("primary"),
                ButtonVariant.Secondary => element.GetCustomHueClass("secondary"),
                ButtonVariant.Warning => element.GetCustomHueClass("warning"),
                ButtonVariant.Danger => element.GetBootstrapVariantClass("danger"),
                _ => element.GetBootstrapVariantClass("primary"),
            }
        );

        return buttonStyleClasses;
    }

    /// <summary>
    /// Applies the appropriate CSS classes to the TagHelperOutput based on the
    /// IButtonStyleable properties.
    /// </summary>
    /// <param name="element">The element implementing IButtonStyleable.</param>
    /// <param name="output">The TagHelperOutput to which the classes will be applied.</param>
    public static void ApplyClasses(this IButtonStyleable element, TagHelperOutput output)
    {
        foreach (var cls in element.BuildClasses())
        {
            if (!string.IsNullOrWhiteSpace(cls))
                output.AddClass(cls, HtmlEncoder.Default);
        }
    }
}
