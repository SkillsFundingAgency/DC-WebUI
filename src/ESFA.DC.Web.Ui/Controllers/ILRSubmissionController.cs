using System;
using System.IO;
using System.Threading.Tasks;
using DC.Web.Authorization.Data.Constants;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.SubmissionService;
using DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DC.Web.Ui.Controllers
{
    [Authorize]
    public class ILRSubmissionController : Controller
    {
        private readonly ISubmissionService _submissionService;

        public ILRSubmissionController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
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

            var fileNameForSubmssion = $" {Path.GetFileNameWithoutExtension(file.FileName).AppendRandomString(5)}.xml";

            var ilrFile = new IlrFileViewModel()
            {
                Filename = file.FileName,
                SubmissionDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time")),
                FileSize = (decimal)file.Length / 1024
            };

            // push file to Storage
            using (var outputStream = await _submissionService.GetBlobStream(fileNameForSubmssion))
            {
                await file.CopyToAsync(outputStream);
            }

            // add to the queue
           var jobId = await _submissionService.SubmitIlrJob(fileNameForSubmssion, User.Ukprn());
            ilrFile.JobId = jobId;

            TempData["ilrSubmission"] = JsonConvert.SerializeObject(ilrFile);
            return RedirectToAction("Index", "Confirmation");
        }
    }
}