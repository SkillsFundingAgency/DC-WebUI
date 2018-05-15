using Newtonsoft.Json;

namespace DC.Web.Ui.Settings.Models
{
    public class CloudStorageSettings : ISettings
    {
        [JsonRequired]
        public string ConnectionString { get; set; }

        [JsonRequired]
        public string ContainerName { get; set; }
    }
}
