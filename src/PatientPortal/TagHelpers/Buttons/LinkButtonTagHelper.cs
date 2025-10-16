using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PatientPortal.TagHelpers.Buttons;

/// <summary>
/// Base class for link button tag helpers <c>&lt;link-button-*&gt;</c>,
/// implementing IButtonStyleable and defining common properties and processing
/// logic for rendering an <c>&lt;a&gt;</c> with button styling.
/// </summary>
public abstract class LinkButtonTagHelperBase(IHtmlGenerator generator)
    : AnchorTagHelper(generator),
        IButtonStyleable
{
    public abstract ButtonVariant Variant { get; }

    [HtmlAttributeName("small")]
    public bool IsSmall { get; set; }

    [HtmlAttributeName("outline")]
    public bool Outline { get; set; }

    [HtmlAttributeName("hover")]
    public HoverEffect Hover { get; set; } = HoverEffect.Lighten;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);

        output.TagName = "a";
        output.TagMode = TagMode.StartTagAndEndTag;

        this.ApplyClasses(output);
    }
}

/// <summary>
/// Renders a link styled like a primary button with the project's styling.
/// <br/>
/// <example>
/// Example:
/// <code>
/// &lt;link-button-primary outline asp-controller="Patient" asp-action="Create"&gt;Input New Patient&lt;/link-button-primary&gt;
/// </code>
/// </example>
/// </summary>
[HtmlTargetElement("link-button-primary")]
public class PrimaryLinkButtonTagHelper(IHtmlGenerator generator)
    : LinkButtonTagHelperBase(generator)
{
    public override ButtonVariant Variant => ButtonVariant.Primary;
}

/// <summary>
/// Renders a link styled like a secondary button with the project's styling.
/// <br/>
/// <example>
/// Example:
/// <code>
/// &lt;link-button-secondary small class="ms-2" asp-controller="Home" asp-action="Index"&gt;Cancel&lt;/link-button-secondary&gt;
/// </code>
/// </example>
/// </summary>
[HtmlTargetElement("link-button-secondary")]
public class SecondaryLinkButtonTagHelper(IHtmlGenerator generator)
    : LinkButtonTagHelperBase(generator)
{
    public override ButtonVariant Variant => ButtonVariant.Secondary;
}
