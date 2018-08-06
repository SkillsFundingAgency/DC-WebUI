using System;
using System.Collections.Generic;
using System.Text;
using DC.Web.Ui.Services.Services;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;
using FluentAssertions;
using Xunit;

namespace DC.Web.Ui.Services.Tests
{
    public class FileNameValidationServiceTests
    {
        //[Theory]
        //[InlineData(".zip")]
        //[InlineData(".xml")]
        //[InlineData(".ZIP")]
        //[InlineData(".XML")]
        //public void IsValidExtension_True(string extension)
        //{
        //    var service = new FileNameValidationService();
        //    service.IsValidExtension($"testfile{extension}").Should().BeTrue();
        //}

        //[Theory]
        //[InlineData(".zip1")]
        //[InlineData(".mov")]
        //[InlineData(".img")]
        //[InlineData(".jpg")]
        //public void IsValidExtension_False(string extension)
        //{
        //    var service = new FileNameValidationService();
        //    service.IsValidExtension($"testfile{extension}").Should().BeFalse();
        //}

        //[Fact]
        //public void IsValidUkprn_True()
        //{
        //    var service = new FileNameValidationService();
        //    service.IsValidUkprn("ILR-10006341-1819-20180118-023456-02.xml", 10006341).Should().BeTrue();
        //}

        //[Fact]
        //public void IsValidUkprn_False()
        //{
        //    var service = new FileNameValidationService();
        //    service.IsValidUkprn("ILR-10006341-1819-20180118-023456-02.xml", 99999999).Should().BeFalse();
        //}

        //[Fact]
        //public void IsValidRegex_True()
        //{
        //    var service = new FileNameValidationService();
        //    service.IsValidRegex("ILR-10006341-1819-20180118-023456-02.xml").Should().BeTrue();
        //}

        //[Fact]
        //public void IsValidRegex_False()
        //{
        //    var service = new FileNameValidationService();
        //    service.IsValidRegex("ILR-10006341-1819-20180118023456-02.xml").Should().BeFalse();
        //}

        //[Fact]
        //public void ValidateFileName_InvalidFileSize()
        //{
        //    var service = new FileNameValidationService();
        //    service.ValidateFileName("ILR-10006341-1819-20180118-023456-02.xml", 0, 10000).Should()
        //        .Be(FileNameValidationResult.EmptyFile);
        //}

        //[Fact]
        //public void ValidateFileName_InvalidUkprn()
        //{
        //    var service = new FileNameValidationService();
        //    service.ValidateFileName("ILR-10006341-1819-20180118-023456-02.xml", 10, 10000).Should()
        //        .Be(FileNameValidationResult.UkprnDifferentToFileName);
        //}

        //[Fact]
        //public void ValidateFileName_InvalidExtension()
        //{
        //    var service = new FileNameValidationService();
        //    service.ValidateFileName("ILR-10006341-1819-20180118-023456-02.x1ml", 10, 10000).Should()
        //        .Be(FileNameValidationResult.InvalidFileExtension);
        //}

        //[Fact]
        //public void ValidateFileName_EmptyfileName()
        //{
        //    var service = new FileNameValidationService();
        //    service.ValidateFileName(null, 10, 10000).Should()
        //        .Be(FileNameValidationResult.EmptyFile);
        //}

        //[Fact]
        //public void ValidateFileName_InvalidaFileNameFormat()
        //{
        //    var service = new FileNameValidationService();
        //    service.ValidateFileName("ILR-10006341-1819-20180118-023456-2.xml", 10, 10000).Should()
        //        .Be(FileNameValidationResult.InvalidFileNameFormat);
        //}

        //[Fact]
        //public void ValidateFileName_Valid()
        //{
        //    var service = new FileNameValidationService();
        //    service.ValidateFileName("ILR-10006341-1819-20180118-023456-02.xml", 10, 10006341).Should()
        //        .Be(FileNameValidationResult.Valid);
        //}
    }
}
