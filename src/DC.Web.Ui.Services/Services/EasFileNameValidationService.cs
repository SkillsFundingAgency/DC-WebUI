﻿using System;
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
    public class EasFileNameValidationService : AbstractFileNameValidationService
    {
        public EasFileNameValidationService([KeyFilter(JobType.EasSubmission)]IKeyValuePersistenceService persistenceService, FeatureFlags featureFlags)
            : base(persistenceService, featureFlags)
        {
        }

        protected override Regex FileNameRegex => new Regex("^(EASDATA)-([1-9][0-9]{7})-((20[0-9]{2})(0[1-9]|1[012])([123]0|[012][1-9]|31))-(([01][0-9]|2[0-3])([0-5][0-9])([0-5][0-9])).((csv)|(CSV))$", RegexOptions.Compiled);

        protected override IEnumerable<string> FileNameExtensions => new List<string>() { ".csv", ".CSV" };

        public override async Task<FileNameValidationResultViewModel> ValidateFileNameAsync(string fileName, long? fileSize, long ukprn)
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

            result = ValidateRegex(fileName, "File name should use the format EASDATA-LLLLLLLL-yyyymmdd-hhmmss.csv");
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

            return new FileNameValidationResultViewModel()
            {
                ValidationResult = FileNameValidationResult.Valid
            };
        }
    }
}