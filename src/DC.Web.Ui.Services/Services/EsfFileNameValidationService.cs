using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Web.Ui.ViewModels;
using ESFA.DC.Web.Ui.ViewModels.Enums;

namespace DC.Web.Ui.Services.Services
{
    public class EsfFileNameValidationService : AbstractFileNameValidationService
    {
        public EsfFileNameValidationService([KeyFilter(JobType.EsfSubmission)]IKeyValuePersistenceService persistenceService, FeatureFlags featureFlags, IJobService jobService)
            : base(persistenceService, featureFlags, jobService)
        {
        }

        protected override Regex FileNameRegex => new Regex("^(SUPPDATA)-([1-9][0-9]{7})-([0-9a-zA-Z-]{1,20})-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9])).((csv)|(CSV))$", RegexOptions.Compiled);

        protected override IEnumerable<string> FileNameExtensions => new List<string>() { ".csv", ".CSV" };

        public override async Task<FileNameValidationResultViewModel> ValidateFileNameAsync(string fileName, long? fileSize, long ukprn, string collectionName)
        {
            var result = ValidateEmptyFile(fileName, fileSize);
            if (result != null)
            {
                return result;
            }

            result = ValidateExtension(fileName, "Your file must be in a CSV format");
            if (result != null)
            {
                return result;
            }

            result = ValidateRegex(fileName, "File name should use the format SUPPDATA-LLLLLLLL-CCCCCCCCCCCCCCCCCCCC-yyyymmdd-hhmmss.csv");
            if (result != null)
            {
                return result;
            }

            result = ValidateUkprn(fileName, ukprn);
            if (result != null)
            {
                return result;
            }

            result = await ValidateUniqueFileAsync(fileName, ukprn);
            if (result != null)
            {
                return result;
            }

            result = LaterFileExists(ukprn, fileName, collectionName);
            if (result != null)
            {
                return result;
            }

            return new FileNameValidationResultViewModel()
            {
                ValidationResult = FileNameValidationResult.Valid
            };
        }

        public override DateTime GetFileDateTime(string fileName)
        {
            var matches = FileNameRegex.Match(fileName);

            return DateTime.ParseExact(
                $"{matches.Groups[4].Value}-{matches.Groups[8].Value}",
                "yyyyMMdd-HHmmss",
                System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
