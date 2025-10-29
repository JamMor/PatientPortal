using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PatientPortal.TagHelpers.Buttons;

/// <summary>
/// Base class for button tag helpers <c>&lt;button-*&gt;</c>, implementing
/// IButtonStyleable and defining common properties and processing logic for
/// rendering a styled <c>&lt;button&gt;</c>.
/// </summary>
public abstract class ButtonTagHelperBase : TagHelper, IButtonStyleable
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
        output.TagName = "button";
        output.TagMode = TagMode.StartTagAndEndTag;

        this.ApplyClasses(output);
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
public class PrimaryButtonTagHelper : ButtonTagHelperBase
{
    public override ButtonVariant Variant => ButtonVariant.Primary;
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
public class SecondaryButtonTagHelper : ButtonTagHelperBase
{
    public override ButtonVariant Variant => ButtonVariant.Secondary;
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
public class WarningButtonTagHelper : ButtonTagHelperBase
{
    public override ButtonVariant Variant => ButtonVariant.Warning;
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
public class DangerButtonTagHelper : ButtonTagHelperBase
{
    public override ButtonVariant Variant => ButtonVariant.Danger;
}
