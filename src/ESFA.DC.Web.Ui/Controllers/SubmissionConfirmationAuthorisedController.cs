using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Route("submission-confirmation")]
    public class SubmissionConfirmationAuthorisedController : BaseAuthorisedController
    {
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly IJobService _jobService;

        public SubmissionConfirmationAuthorisedController(
            ICollectionManagementService collectionManagementService,
            IJobService jobService,
            ILogger logger)
            : base(logger)
        {
            _collectionManagementService = collectionManagementService;
            _jobService = jobService;
        }

        [HttpGet]
        [Route("{jobId}/{isCleanFile?}")]
        public async Task<IActionResult> Index(long jobId, bool isCleanFile = false)
        {
            if (isCleanFile)
            {
                ViewData[ViewDataConstants.IsCleanFile] = true;
            }

            var data = await _jobService.GetConfirmation(Ukprn, jobId);

            await SetupNextPeriod(data?.CollectionName);

            return View(data);
        }

        [HttpGet]
        [Route("{jobId}/hide-feedback")]
        public async Task<IActionResult> HideFeedback(long jobId)
        {
            ViewData[ViewDataConstants.IsFeedbackHidden] = true;

            var data = await _jobService.GetConfirmation(Ukprn, jobId);

            await SetupNextPeriod(data?.CollectionName);

            return View("Index", data);
        }

        private async Task SetupNextPeriod(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                return;
            }

            if (await _collectionManagementService.GetCurrentPeriodAsync(collectionName) == null)
            {
                Logger.LogWarning($"No active period for collection : {collectionName}");

                var nextPeriod = await _collectionManagementService.GetNextPeriodAsync(collectionName);
                ViewData[ViewDataConstants.NextReturnOpenDate] = nextPeriod?.NextOpeningDate;
            }
        }
    }
}