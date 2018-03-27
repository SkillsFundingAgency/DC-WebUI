using DC.Web.Ui.ClaimTypes;
using DC.Web.Ui.Models;
using DC.Web.Ui.Services.ServiceBus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using DC.Web.Ui.Extensions;
using DC.Web.Ui.Services.SubmissionService;
using DC.Web.Ui.Settings.Models;
using DC.Web.Ui.ViewModels;

namespace DC.Web.Ui.Controllers
{
    [Authorize(Policy = PolicyTypes.FileSubmission)]
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
        [RequestSizeLimit(500_000_000)]
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
            var correlationId = Guid.NewGuid();

            var ilrFile = new IlrFileViewModel()
            {
                Filename = file.FileName,
                SubmissionDateTime = DateTime.Now,
                FileSize =(decimal)file.Length /1024,
                CorrelationId = correlationId
            };
           
            //push file to Storage
            using (var outputStream = await _submissionService.GetBlobStream(fileNameForSubmssion))
            {
                await file.CopyToAsync(outputStream);
            }

            //add to the queue
            await _submissionService.AddMessageToQueue(fileNameForSubmssion, correlationId);

            TempData["ilrSubmission"] = JsonConvert.SerializeObject(ilrFile);
            return RedirectToAction("Index","Confirmation");
        }

    }
}