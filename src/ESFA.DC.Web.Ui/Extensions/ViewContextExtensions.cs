using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DC.Web.Ui.Extensions
{
    public static class ViewContextExtensions
    {
        public static string GetGroupItemStyle(this ViewContext context, string fieldName)
        {
            if (!string.IsNullOrEmpty(context.ModelState[fieldName]?.Errors?.FirstOrDefault()?.ErrorMessage))
            {
                return "govuk-form-group  govuk-form-group--error";
            }

            return "govuk-form-group";
        }
    }
}
