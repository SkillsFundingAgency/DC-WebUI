using System;
using System.Collections.Generic;
using System.IO;
using DC.Web.Ui.Areas.ILR.Controllers;
using DC.Web.Ui.Services.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
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

            modelresult.ReportFileSize.Should().Be("20.30");
            modelresult.CollectionName.Should().Be("ILR1819");
            modelresult.TotalErrors.Should().Be(100);
            modelresult.TotalErrorLearners.Should().Be(20);
            modelresult.TotalLearners.Should().Be(50);
            modelresult.TotalWarningLearners.Should().Be(30);
            modelresult.TotalWarnings.Should().Be(40);
        }

        [Fact]
        public void Download_Test()
        {
            var controller = GetController();
            var result = controller.Download(It.IsAny<long>()).Result;

            result.Should().BeOfType(typeof(FileStreamResult));
        }

        private ValidationResultsController GetController()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                ["JobId"] = 1000
            };

            var validationErrorsServiceMock = new Mock<IValidationResultsService>();
            var submissionServiceMock = new Mock<ISubmissionService>();
            var reportServiceMock = new Mock<IStorageService>();

            submissionServiceMock.Setup(x => x.GetJob(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(() => new FileUploadJob()
            {
                JobId = 1000,
                Ukprn = 0,
                CollectionName = "ILR1819"
            });

            validationErrorsServiceMock.Setup(x => x.GetValidationResult(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<JobType>(), It.IsAny<DateTime>()))
                .ReturnsAsync(() => new ValidationResultViewModel()
                {
                    JobId = 1000,
                    ReportFileSize = "20.30",
                    CollectionName = "ILR1819",
                    TotalErrors = 100,
                    TotalErrorLearners = 20,
                    TotalLearners = 50,
                    TotalWarningLearners = 30,
                    TotalWarnings = 40
                });

            reportServiceMock.Setup(x => x.GetBlobFileStreamAsync(It.IsAny<string>(), It.IsAny<JobType>()))
                .ReturnsAsync(() => new MemoryStream());

            var controller = new ValidationResultsController(
                validationErrorsServiceMock.Object,
                submissionServiceMock.Object,
                reportServiceMock.Object,
                new Mock<ICollectionManagementService>().Object,
                new Mock<ILogger>().Object)
            {
                TempData = tempData
            };
            return controller;
        }
    }
}
