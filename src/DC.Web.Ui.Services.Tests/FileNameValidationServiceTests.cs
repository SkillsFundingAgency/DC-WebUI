using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DC.Web.Ui.Services.Services;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace DC.Web.Ui.Services.Tests
{
    public class FileNameValidationServiceTests
    {
        [Theory]
        [InlineData(".zip")]
        [InlineData(".xml")]
        [InlineData(".ZIP")]
        [InlineData(".XML")]
        public void IsValidExtension_True(string extension)
        {
            var service = new FileNameValidationService(new Mock<IKeyValuePersistenceService>().Object);
            service.IsValidExtension($"testfile{extension}").Should().BeTrue();
        }

        [Theory]
        [InlineData(".zip1")]
        [InlineData(".mov")]
        [InlineData(".img")]
        [InlineData(".jpg")]
        public void IsValidExtension_False(string extension)
        {
            var service = new FileNameValidationService(new Mock<IKeyValuePersistenceService>().Object);
            service.IsValidExtension($"testfile{extension}").Should().BeFalse();
        }

        [Fact]
        public void IsValidUkprn_True()
        {
            var service = new FileNameValidationService(new Mock<IKeyValuePersistenceService>().Object);
            service.IsValidUkprn("ILR-10006341-1819-20180118-023456-02.xml", 10006341).Should().BeTrue();
        }

        [Fact]
        public void IsValidUkprn_False()
        {
            var service = new FileNameValidationService(new Mock<IKeyValuePersistenceService>().Object);
            service.IsValidUkprn("ILR-10006341-1819-20180118-023456-02.xml", 99999999).Should().BeFalse();
        }

        [Fact]
        public void IsValidRegex_True()
        {
            var service = new FileNameValidationService(new Mock<IKeyValuePersistenceService>().Object);
            service.IsValidRegex("ILR-10006341-1819-20180118-023456-02.xml").Should().BeTrue();
        }

        [Fact]
        public void IsValidRegex_False()
        {
            var service = new FileNameValidationService(new Mock<IKeyValuePersistenceService>().Object);
            service.IsValidRegex("ILR-10006341-1819-20180118023456-02.xml").Should().BeFalse();
        }

        [Fact]
        public void ValidateFileName_InvalidFileSize()
        {
            var service = new FileNameValidationService(new Mock<IKeyValuePersistenceService>().Object);
            service.ValidateFileNameAsync("ILR-10006341-1819-20180118-023456-02.xml", 0, 10000).Result.ValidationResult.Should()
                .Be(FileNameValidationResult.EmptyFile);
        }

        //[Fact]
        //public void ValidateFileName_InvalidUkprn()
        //{
        //    var service = new FileNameValidationService(new Mock<IKeyValuePersistenceService>().Object);
        //    service.ValidateFileNameAsync("ILR-10006341-1819-20180118-023456-02.xml", 10, 10000).Result.ValidationResult.Should()
        //        .Be(FileNameValidationResult.UkprnDifferentToFileName);
        //}

        [Fact]
        public void ValidateFileName_InvalidExtension()
        {
            var service = new FileNameValidationService(new Mock<IKeyValuePersistenceService>().Object);
            service.ValidateFileNameAsync("ILR-10006341-1819-20180118-023456-02.x1ml", 10, 10000).Result.ValidationResult.Should()
                .Be(FileNameValidationResult.InvalidFileExtension);
        }

        [Fact]
        public void ValidateFileName_EmptyfileName()
        {
            var service = new FileNameValidationService(new Mock<IKeyValuePersistenceService>().Object);
            service.ValidateFileNameAsync(null, 10, 10000).Result.ValidationResult.Should()
                .Be(FileNameValidationResult.EmptyFile);
        }

        //[Fact]
        //public void ValidateFileName_InvalidaFileNameFormat()
        //{
        //    var service = new FileNameValidationService(new Mock<IKeyValuePersistenceService>().Object);
        //    service.ValidateFileNameAsync("ILR-10006341-1819-20180118-023456-2.xml", 10, 10000).Result.ValidationResult.Should()
        //        .Be(FileNameValidationResult.InvalidFileNameFormat);
        //}

        [Fact]
        public void ValidateFileName_Valid()
        {
            var service = new FileNameValidationService(new Mock<IKeyValuePersistenceService>().Object);
            service.ValidateFileNameAsync("ILR-10006341-1819-20180118-023456-02.xml", 10, 10006341).Result.ValidationResult.Should()
                .Be(FileNameValidationResult.Valid);
        }

        //[Fact]
        //public void ValidateFileName_FileAlredyExists()
        //{
        //    var mockStorageService = new Mock<IKeyValuePersistenceService>();
        //    mockStorageService.Setup(x => x.ContainsAsync(It.IsAny<string>(), default(CancellationToken)))
        //        .ReturnsAsync(() => true);

        //    var service = new FileNameValidationService(mockStorageService.Object);
        //    service.ValidateFileNameAsync("ILR-10006341-1819-20180118-023456-02.xml", 10, 10006341).Result.ValidationResult.Should()
        //        .Be(FileNameValidationResult.FileAlreadyExists);
        //}
    }
}
