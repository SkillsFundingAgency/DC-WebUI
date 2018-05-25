using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using DC.Web.Ui.Base;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.Models;
using DC.Web.Ui.Services.ValidationErrors;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace DC.Web.Ui.Controllers
{
    public class ValidationErrorsController : BaseController
    {
        private readonly IValidationErrorsService _validationErrorsService;

        public ValidationErrorsController(IValidationErrorsService validationErrorsService, AuthenticationSettings authenticationSettings)
            : base(authenticationSettings)
        {
            _validationErrorsService = validationErrorsService;
        }

        public async Task<IActionResult> Index(long jobId)
        {
            ViewBag.JobId = jobId;
            return View(await _validationErrorsService.GetValidationErrors(Ukprn, jobId));
        }

        public async Task<FileResult> Download(long jobId)
        {
            var data = await _validationErrorsService.GetValidationErrors(Ukprn, jobId);
            var stream = new MemoryStream();
            var csvWriter = new StreamWriter(stream, Encoding.GetEncoding("shift-jis"));
            var csv = new CsvWriter(csvWriter);
            csv.WriteRecords(data);
            csvWriter.Flush();
            csv.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "text/csv");
        }
    }
}