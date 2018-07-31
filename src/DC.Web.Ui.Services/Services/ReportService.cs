using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Interfaces;
using DC.Web.Ui.Services.ViewModels;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.ILR.ValidationErrors.Interface.Models;
using ESFA.DC.Serialization.Interfaces;

namespace DC.Web.Ui.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly IJsonSerializationService _serializationService;

        public ReportService(IBespokeHttpClient httpClient, ApiSettings apiSettings, IJsonSerializationService serializationService)
        {
            _httpClient = httpClient;
            _baseUrl = apiSettings.JobQueueBaseUrl;
            _serializationService = serializationService;
        }

        public Task<ValidationReport> GetValidationReport(long ukprn, long jobId)
        {
           throw new NotImplementedException();
        }
    }
}
