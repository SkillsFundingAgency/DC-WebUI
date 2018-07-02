using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.SubmissionService;
using DC.Web.Ui.Services.ValidationErrors;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DC.Web.Ui.Controllers
{
    [Route("validation-results")]
    public class ValidationResultsController : BaseController
    {
        private readonly IValidationErrorsService _validationErrorsService;
        private readonly ISubmissionService _submissionService;

        public ValidationResultsController(IValidationErrorsService validationErrorsService, ISubmissionService submissionService, AuthenticationSettings authenticationSettings)
            : base(authenticationSettings)
        {
            _validationErrorsService = validationErrorsService;
            _submissionService = submissionService;
        }

        [Route("")]
        public async Task<IActionResult> Index(long jobId)
        {
            ViewBag.JobId = jobId;

            var job = await _submissionService.GetJob(User.Ukprn(), jobId);
            if (job == null)
            {
                return View(new ValidationResultViewModel());
            }

            var result = new ValidationResultViewModel
            {
                JobId = jobId,
                FileSize = job.FileSize / 1024,
                Filename = job.FileName,
                SubmissionDateTime = job.DateTimeSubmittedUtc,
                TotalLearners = job.TotalLearners,
                UploadedBy = job.SubmittedBy
            };

            return View(result);
        }

        [HttpPost]
        public IActionResult Submit(bool submitFile, long jobId, int totalLearners)
        {
            if (!submitFile)
            {
                _submissionService.UpdateJobStatus(jobId, JobStatusType.Completed, totalLearners);
                return RedirectToAction("Index", "SubmissionOptions");
            }
            else
            {
                _submissionService.UpdateJobStatus(jobId, JobStatusType.Ready, totalLearners);
                return RedirectToAction("Index", "Confirmation");
            }
        }

        [Route("Download")]
        public async Task<FileResult> Download(long jobId)
        {
            //TODO:This will be removed/refactored based on actual report requirement
            var data = await _validationErrorsService.GetValidationErrors(Ukprn, jobId);
            var stream = new MemoryStream();
            var csvWriter = new StreamWriter(stream, Encoding.GetEncoding("shift-jis"));
            var csv = new CsvWriter(csvWriter);
            csv.WriteRecords(data);
            csvWriter.Flush();
            csv.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "text/csv", $"{Ukprn}_{jobId}_ValidationErrors.csv");
        }
    }
}