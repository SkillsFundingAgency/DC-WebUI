using Newtonsoft.Json;

namespace DC.Web.Ui.Settings.Models
{
    public class ServiceBusQueueSettings
    {
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string ConnectionString { get; set; }
    }
}