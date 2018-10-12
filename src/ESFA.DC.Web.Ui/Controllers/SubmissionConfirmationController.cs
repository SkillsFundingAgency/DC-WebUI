using System.Threading.Tasks;
using DC.Web.Ui.Base;
using DC.Web.Ui.Constants;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DC.Web.Ui.Controllers
{
    [Route("submission-confirmation")]
    public class SubmissionConfirmationController : BaseController
    {
        private readonly ICollectionManagementService _collectionManagementService;
        private readonly ISubmissionService _submissionService;

        public SubmissionConfirmationController(
            ICollectionManagementService collectionManagementService,
            ISubmissionService submissionService,
            ILogger logger)
            : base(logger)
        {
            _collectionManagementService = collectionManagementService;
            _submissionService = submissionService;
        }

        [HttpGet]
        [Route("{jobId}")]
        public async Task<IActionResult> Index(long jobId)
        {
            var data = await _submissionService.GetConfirmation(Ukprn, jobId);

            await SetupNextPeriod(data?.CollectionName);

            return View(data);
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