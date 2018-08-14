using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DC.Web.Ui.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "asp-valid")]
    public class ErrorStyleTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-valid")]
        public bool IsValid { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var existingClass = output.Attributes.FirstOrDefault(a => a.Name == "class")?.Value;
            if (!IsValid)
            {
                output.Attributes.SetAttribute("class", $"{existingClass} govuk-form-group--error");
            }
        }
    }
}
