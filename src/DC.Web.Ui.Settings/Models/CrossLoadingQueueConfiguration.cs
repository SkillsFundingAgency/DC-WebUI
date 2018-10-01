using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.Queueing.Interface.Configuration;
using Newtonsoft.Json;

namespace DC.Web.Ui.Settings.Models
{
    public class CrossLoadingQueueConfiguration : IQueueConfiguration
    {
        [JsonRequired]
        public string ConnectionString { get; set; }

        [JsonRequired]
        public string QueueName { get; set; }

        public int MaxConcurrentCalls => 1;

        public int MinimumBackoffSeconds => 2;

        public int MaximumBackoffSeconds => 5;

        public int MaximumRetryCount => 3;

        public TimeSpan MaximumCallbackTimeSpan => new TimeSpan(0, 10, 0);
    }
}
