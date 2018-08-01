using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using DC.Web.Ui.Controllers;
using DC.Web.Ui.Controllers.IlrSubmission;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.DateTime.Provider.Interface;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace DC.Web.Ui.Tests.Controllers
{
    public class ValidationResultsControllerTests
    {
        [Fact]
        public void Index_Test()
        {
            var controller = GetController();
            var result = controller.Index(1000).Result;

            result.Should().BeOfType(typeof(ViewResult));
            var modelresult = (ValidationResultViewModel)((ViewResult)result).Model;

            modelresult.JobId.Should().Be(1000);
        }

        [Fact]
        public void Download_Test()
        {
            var controller = GetController();
            var result = controller.Download().Result;

            result.Should().BeOfType(typeof(FileStreamResult));
        }

        private ValidationResultsController GetController()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["JobId"] = 1000
            };

            var validationErrorsServiceMock = new Mock<IValidationErrorsService>();
            var submissionServiceMock = new Mock<ISubmissionService>();
            var reportServiceMock = new Mock<IReportService>();

            submissionServiceMock.Setup(x => x.GetJob(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(() => new IlrJob
                {
                    JobId = 1000,
                    TotalLearners = 10
                });

            validationErrorsServiceMock.Setup(x => x.GetValidationErrors(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(
                    () => new List<ValidationErrorDto>());

            reportServiceMock.Setup(x => x.GetReportStreamAsync(It.IsAny<string>()))
                .ReturnsAsync(() => new MemoryStream());

            var controller = new ValidationResultsController(
                validationErrorsServiceMock.Object,
                submissionServiceMock.Object,
                reportServiceMock.Object,
                new Mock<ILogger>().Object)
            {
                TempData = tempData
            };
            return controller;
        }
    }
}
