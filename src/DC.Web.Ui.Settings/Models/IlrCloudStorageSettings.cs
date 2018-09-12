using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using Newtonsoft.Json;

namespace DC.Web.Ui.Settings.Models
{
    public class IlrCloudStorageSettings : ISettings, IAzureStorageKeyValuePersistenceServiceConfig
    {
        [JsonRequired]
        public string ConnectionString { get; set; }

        [JsonRequired]
        public string ContainerName { get; set; }
    }
}
