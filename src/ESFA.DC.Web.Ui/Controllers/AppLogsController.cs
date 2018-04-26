using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Services.AppLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Authorize]
    public class AppLogsController : Controller
    {
        private readonly IAppLogsReader _appLogsReader;

        public AppLogsController(IAppLogsReader appLogsReader)
        {
            _appLogsReader = appLogsReader;
        }

        public IActionResult Index(long jobId)
        {
            return View(_appLogsReader.GetApplicationLogs(jobId));
        }
    }
}