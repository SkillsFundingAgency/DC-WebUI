﻿using Newtonsoft.Json;

namespace DC.Web.Ui.Settings.Models
{
    public class AuthenticationSettings : ISettings
    {
        [JsonRequired]
        public string WtRealm { get; set; }

        [JsonRequired]
        public string MetadataAddress { get; set; }
    }
}