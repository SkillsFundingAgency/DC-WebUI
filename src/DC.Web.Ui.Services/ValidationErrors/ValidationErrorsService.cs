﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DC.Web.Ui.Services.BespokeHttpClient;
using DC.Web.Ui.Services.Models;
using DC.Web.Ui.Settings.Models;
using ESFA.DC.Serialization.Interfaces;
using Newtonsoft.Json;

namespace DC.Web.Ui.Services.ValidationErrors
{
    public class ValidationErrorsService : IValidationErrorsService
    {
        private readonly IBespokeHttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ISerializationService _serializationService;

        public ValidationErrorsService(IBespokeHttpClient httpClient, JobQueueApiSettings apiSettings, ISerializationService serializationService)
        {
            _httpClient = httpClient;
            _baseUrl = apiSettings.BaseUrl;
            _serializationService = serializationService;
        }

        public async Task<IEnumerable<ValidationError>> GetValidationErrors(long jobId)
        {
            var data = await _httpClient.GetDataAsync($"{_baseUrl}/validationerrors/{jobId}");
            return _serializationService.Deserialize<IEnumerable<ValidationError>>(data);
        }
    }
}