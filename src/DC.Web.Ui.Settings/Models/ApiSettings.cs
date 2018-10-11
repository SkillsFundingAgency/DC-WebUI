using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DC.Web.Ui.Settings.Models
{
    public class ApiSettings : ISettings
    {
        [JsonRequired]
        public string JobManagementApiBaseUrl { get; set; }
    }
}
