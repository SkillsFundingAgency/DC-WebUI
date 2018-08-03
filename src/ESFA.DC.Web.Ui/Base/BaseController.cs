using DC.Web.Authorization.Data.Constants;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Base
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected BaseController(ILogger logger)
        {
            Logger = logger;
        }

        protected ILogger Logger { get; set; }

        protected long Ukprn => User.Ukprn();

        protected long ContextJobId
        {
            get
            {
                if (TempData.ContainsKey("JobId"))
                {
                    return long.Parse(TempData["JobId"].ToString());
                }

                return 0;
            }
        }

        protected void SetJobId(long jobId)
        {
            TempData["JobId"] = jobId;
        }
    }
}
