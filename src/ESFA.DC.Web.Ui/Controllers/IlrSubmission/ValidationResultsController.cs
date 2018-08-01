using System.Linq;
using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.ViewModels;
using ESFA.DC.JobStatus.Interface;
using ESFA.DC.KeyGenerator.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers.IlrSubmission
{
    [Route("validation-results")]
    public class ValidationResultsController : BaseController
    {
        private readonly IValidationErrorsService _validationErrorsService;
        private readonly ISubmissionService _submissionService;
        private readonly IReportService _reportService;
        private readonly IKeyGenerator _keyGenerator;

        public ValidationResultsController(IValidationErrorsService validationErrorsService, ISubmissionService submissionService, IReportService reportService, IKeyGenerator keyGenerator)
        {
            _validationErrorsService = validationErrorsService;
            _submissionService = submissionService;
            _reportService = reportService;
            _keyGenerator = keyGenerator;
        }

        [Route("")]
        public async Task<IActionResult> Index(long jobId)
        {
            SetJobId(jobId);

            var job = await _submissionService.GetJob(User.Ukprn(), jobId);
            if (job == null)
            {
                return View(new ValidationResultViewModel());
            }

            var valErrors = await _validationErrorsService.GetValidationErrors(Ukprn, jobId);

            var result = new ValidationResultViewModel
            {
                JobId = jobId,
                FileSize = job.FileSize / 1024,
                Filename = job.FileName,
                SubmissionDateTime = job.DateTimeSubmittedUtc,
                TotalLearners = job.TotalLearners,
                UploadedBy = job.SubmittedBy,
                TotalErrors = valErrors.Count()
            };

            return View(result);
        }

        [HttpPost]
        public IActionResult Submit(bool submitFile, int totalLearners)
        {
            if (!submitFile)
            {
                _submissionService.UpdateJobStatus(ContextJobId, JobStatusType.Completed, totalLearners);
                return RedirectToAction("Index", "SubmissionOptions");
            }
            else
            {
                _submissionService.UpdateJobStatus(ContextJobId, JobStatusType.Ready, totalLearners);
                return RedirectToAction("Index", "Confirmation");
            }
        }

        [Route("Download")]
        public async Task<FileResult> Download()
        {
            var validationErrorsKey = $"{User.Ukprn()}/{ContextJobId}/{TaskKeys.ValidationErrors}.csv";
            var csvBlobStream = await _reportService.GetReportStreamAsync(validationErrorsKey);
            return File(csvBlobStream, "text/csv", $"{Ukprn}_{ContextJobId}_ValidationErrors.csv");
        }
    }
}