using System;
using System.IO;
using System.Threading.Tasks;
using DC.Web.Authorization.Data.Constants;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.SubmissionService;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DC.Web.Ui.Controllers
{
    public class ILRSubmissionController : BaseController
    {
        private readonly ISubmissionService _submissionService;
        private readonly ILogger _logger;

        public ILRSubmissionController(ISubmissionService submissionService, ILogger logger, AuthenticationSettings authenticationSettings)
            : base(authenticationSettings)
        {
            _submissionService = submissionService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [RequestSizeLimit(524_288_000)]
        [Authorize(Policy = FeatureNames.FileSubmission)]
        public async Task<IActionResult> Submit(IFormFile file)
        {
            if (file == null)
            {
                return Index();
            }

            if (file.Length == 0)
            {
                return Index();
            }

            try
            {
                var ilrFile = new IlrFileViewModel()
                {
                    Filename = file.FileName,
                    SubmissionDateTime = TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")),
                    FileSize = (decimal)file.Length / 1024
                };

                // push file to Storage
                using (var outputStream = await _submissionService.GetBlobStream(file.FileName))
                {
                    await file.CopyToAsync(outputStream);
                }

                // add to the queue
                var jobId = await _submissionService.SubmitIlrJob(file.FileName, Ukprn);
                ilrFile.JobId = jobId;

                TempData["ilrSubmission"] = JsonConvert.SerializeObject(ilrFile);
                return RedirectToAction("Index", "Confirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error trying to subnmit ILR file with name : {file.FileName}", ex);
                return View("Error", new ErrorViewModel());
            }
        }
    }
}