using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DC.Web.Ui.TagHelpers
{
    [HtmlTargetElement("span")]
    public class ReturnPeriodTagHelper : TagHelper
    {
        private readonly ICollectionManagementService _collectionManagementService;

        public ReturnPeriodTagHelper(ICollectionManagementService collectionManagementService)
        {
            _collectionManagementService = collectionManagementService;
        }

        [HtmlAttributeName("collection")]
        public string Collection { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(Collection))
            {
                var period = await _collectionManagementService.GetCurrentPeriodAsync(Collection);
                output.Content.SetHtmlContent($"Return Period {period?.PeriodNumber} ({period?.PeriodName()})");
            }
        }
    }
}
