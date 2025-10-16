using System.Collections.Generic;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PatientPortal.TagHelpers;

/// <summary>
/// Base tag helper for rendering a styled <c>&lt;button&gt;</c> element using project button conventions.
/// Handles class generation and merging for consistent button styling.
/// </summary>
public abstract class BaseButtonTagHelper : TagHelper
{
    /// <summary>
    /// The button visual style. "primary" (default), "secondary", or "danger".
    /// </summary>
    protected abstract string Variant { get; }

    /// <summary>
    /// If true, applies the <c>btn-sm</c> Bootstrap size modifier.
    /// </summary>
    [HtmlAttributeName("small")]
    public bool IsSmall { get; set; }

    /// <summary>
    /// If true, uses the outline variant for the button style.
    /// For "primary" and "secondary", applies a custom outline class; for others, uses Bootstrap outline classes.
    /// </summary>
    [HtmlAttributeName("outline")]
    public bool Outline { get; set; }

    [HtmlAttributeName("hover")]
    public string Hover { get; set; } = "lighten";

    /// <summary>
    /// The HTML <c>type</c> attribute. Defaults to "button".
    /// </summary>
    [HtmlAttributeName("type")]
    public string? Type { get; set; }

    private string GetHoverModifierClass()
    {
        return Hover switch
        {
            "lighten" => "hover-lighten",
            "darken" => "hover-darken",
            _ => string.Empty,
        };
    }

    private List<string> GetCustomHueClass(string color)
    {
        List<string> hueClasses = color switch
        {
            "primary" => ["hue-primary", "and-dimmed"],
            "secondary" => ["hue-secondary"],
            "warning" => ["hue-warning"],
            _ => [],
        };

        List<string> outlineClass = Outline ? ["on-btn-outline"] : [GetHoverModifierClass(), "on-btn"];
        hueClasses.AddRange(outlineClass);
        
        return hueClasses;
    }

    private List<string> GetBootstrapVariantClass(string color)
    {
        return Outline ? [$"btn-outline-{color}"] : [$"btn-{color}"];
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "button";
        output.TagMode = TagMode.StartTagAndEndTag;

        output.Attributes.SetAttribute("type", Type);

        List<string> buttonStyleClasses = ["btn"];

        if (IsSmall)
        {
            buttonStyleClasses.Add("btn-sm");
        }

        buttonStyleClasses.AddRange(Variant switch
        {
            "secondary" => GetCustomHueClass("secondary"),
            "warning" => GetCustomHueClass("warning"),
            "danger" => GetBootstrapVariantClass("danger"),
            _ => GetCustomHueClass("primary"),
        });
        
        foreach (var cls in buttonStyleClasses)
        {
            output.AddClass(cls, HtmlEncoder.Default);
        }
    }
}

/// <summary>
/// Renders a primary button with the project's styling.
/// <br/>
/// <example>
/// Example:
/// <code>
/// &lt;button-primary type="submit"&gt;Save&lt;/button-primary&gt;
/// </code>
/// </example>
/// </summary>
[HtmlTargetElement("button-primary")]
public class PrimaryButtonTagHelper : BaseButtonTagHelper
{
    protected override string Variant => "primary";
}

/// <summary>
/// Renders a secondary button with the project's styling.
/// <br/>
/// <example>
/// Example:
/// <code>
/// &lt;button-secondary small class="ms-2"&gt;Cancel&lt;/button-secondary&gt;
/// </code>
/// </example>
/// </summary>
[HtmlTargetElement("button-secondary")]
public class SecondaryButtonTagHelper : BaseButtonTagHelper
{
    protected override string Variant => "secondary";
}

/// <summary>
/// Renders a warning button with the project's styling.
/// <br/>
/// <example>
/// Example:
/// <code>
/// &lt;button-warning outline&gt;Archive&lt;/button-warning&gt;
/// </code>
/// </example>
/// </summary>
[HtmlTargetElement("button-warning")]
public class WarningButtonTagHelper : BaseButtonTagHelper
{
    protected override string Variant => "warning";
}

/// <summary>
/// Renders a danger button with the project's styling.
/// <br/>
/// <example>
/// Example:
/// <code>
/// &lt;button-danger small outline&gt;Delete&lt;/button-danger&gt;
/// </code>
/// </example>
/// </summary>
[HtmlTargetElement("button-danger")]
public class DangerButtonTagHelper : BaseButtonTagHelper
{
    protected override string Variant => "danger";
}
