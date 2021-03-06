﻿using DC.Web.Authorization;
using DC.Web.Authorization.Data.Constants;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Extensions;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Base
{
    [Authorize(Policy = PolicyTypes.FileSubmission)]
    public abstract class BaseAuthorisedController : Controller
    {
        protected BaseAuthorisedController(ILogger logger)
        {
            Logger = logger;
        }

        protected bool IsHelpSectionHidden
        {
            set => ViewData[ViewDataConstants.IsHelpSectionHidden] = value;
        }

        protected ILogger Logger { get; set; }

        protected long Ukprn => User.Ukprn();

        protected void AddError(string key)
        {
            ModelState.AddModelError(key, ErrorMessageLookup.GetErrorMessage(key));
        }

        protected void AddError(string key, string message)
        {
            ModelState.AddModelError(key, message);
        }
    }
}
