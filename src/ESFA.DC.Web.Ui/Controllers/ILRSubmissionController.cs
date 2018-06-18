using System;
using System.IO;
using System.Threading.Tasks;
using DC.Web.Authorization.Data.Constants;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.SubmissionService;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using ESFA.DC.DateTime.Provider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DC.Web.Ui.Controllers
{
    [Route("ilr-submission")]
    public class ILRSubmissionController : BaseController
    {
        private readonly ISubmissionService _submissionService;
        private readonly ILogger _logger;
        private readonly IJsonSerializationService _serializationService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ILRSubmissionController(ISubmissionService submissionService, ILogger logger, AuthenticationSettings authenticationSettings, IJsonSerializationService serializationService, IDateTimeProvider dateTimeProvider)
            : base(authenticationSettings)
        {
            _submissionService = submissionService;
            _logger = logger;
            _serializationService = serializationService;
            _dateTimeProvider = dateTimeProvider;
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
                    SubmissionDateTime = _dateTimeProvider.GetNowUtc(),
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

                TempData["ilrSubmission"] = _serializationService.Serialize(ilrFile);
                return RedirectToAction("Index", "InProgress");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error trying to subnmit ILR file with name : {file.FileName}", ex);
                return View("Error", new ErrorViewModel());
            }
        }
    }
}