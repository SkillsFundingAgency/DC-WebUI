﻿using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Services.SubmissionService;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    public class HistoryController : BaseController
    {
        private readonly ISubmissionService _submissionService;

        public HistoryController(ISubmissionService submissionService, AuthenticationSettings authenticationSettings)
            : base(authenticationSettings)
        {
            _submissionService = submissionService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _submissionService.GetAllJobs(Ukprn);
            return View(result);
        }
    }
}