using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DC.Web.Ui.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "error-field-name")]
    public class FieldErrorTagHelper : TagHelper
    {
        [HtmlAttributeName("error-field-name")]
        public string ErrorFieldName { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var errorMessage = ViewContext.ModelState.ContainsKey(ErrorFieldName) ?
                ViewContext.ModelState[ErrorFieldName]?.Errors?.First()?.ErrorMessage :
                string.Empty;

            if (string.IsNullOrEmpty(errorMessage))
            {
                output.SuppressOutput();
            }
            else
            {
                output.Content.SetHtmlContent($"<span class='govuk-error-message'>{errorMessage}</span>");
            }
        }
    }
}
