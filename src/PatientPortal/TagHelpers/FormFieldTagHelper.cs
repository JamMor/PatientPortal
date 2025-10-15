using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PatientPortal.TagHelpers;

/// <summary>
/// Renders a Bootstrap form-group containing a label, client-side validation message, and an
/// input control for the given model property. The control type (text, password, date, textarea)
/// is inferred from DataType annotations on the property but can be overridden with the
/// <c>type</c> attribute.
/// 
/// <example>
///     <div class="mb-3">
///         <label class="form-label" asp-for="LastName"></label>
///         <span class="text-danger" asp-validation-for="LastName"></span>
///         <input class="form-control" asp-for="LastName">
///     </div>
/// </example>
/// </summary>
[HtmlTargetElement("form-field")]
public class FormFieldTagHelper : TagHelper
{
    private readonly IHtmlGenerator _generator;

    [HtmlAttributeName("asp-for")]
    public ModelExpression For { get; set; } = default!;

    /// <summary>
    /// Overrides the inferred input type.
    /// Accepted values: "text" (default), "textarea", "password", "date".
    /// </summary>
    [HtmlAttributeName("type")]
    public string? InputType { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;

    public FormFieldTagHelper(IHtmlGenerator generator)
    {
        _generator = generator;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.SetAttribute("class", "mb-3");
        output.TagMode = TagMode.StartTagAndEndTag;

        var label = _generator.GenerateLabel(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            labelText: null,
            htmlAttributes: new { @class = "form-label" });

        var validation = _generator.GenerateValidationMessage(
            ViewContext,
            For.ModelExplorer,
            For.Name,
            message: null,
            tag: "span",
            htmlAttributes: new { @class = "text-danger" });

        var resolvedType = ResolveInputType();

        TagBuilder inputControl;
        if (resolvedType == "textarea")
        {
            inputControl = _generator.GenerateTextArea(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                rows: 0,
                columns: 0,
                htmlAttributes: new { @class = "form-control" });
        }
        else
        {
            object htmlAttributes = resolvedType switch
            {
                "password" => new { @class = "form-control", type = "password" },
                "date" => new { @class = "form-control", type = "date" },
                _ => new { @class = "form-control" },
            };

            string? format = resolvedType == "date" ? "{0:yyyy-MM-dd}" : null;

            inputControl = _generator.GenerateTextBox(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                value: For.Model,
                format: format,
                htmlAttributes: htmlAttributes);
        }

        output.Content
            .AppendHtml(label)
            .AppendHtml(validation)
            .AppendHtml(inputControl);
    }

    private string ResolveInputType()
    {
        if (InputType is not null)
            return InputType;

        return For.ModelExplorer.Metadata.DataTypeName switch
        {
            "Password" => "password",
            "Date" => "date",
            "MultilineText" => "textarea",
            _ => "text",
        };
    }
}
