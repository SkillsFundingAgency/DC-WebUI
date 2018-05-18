using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DC.Web.Ui.Settings.Models
{
    public class JobQueueApiSettings : ISettings
    {
        [JsonRequired]
        public string BaseUrl { get; set; }
    }
}
