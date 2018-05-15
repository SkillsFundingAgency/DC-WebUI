using Newtonsoft.Json;

namespace DC.Web.Ui.Settings.Models
{
    public class ConnectionStrings : ISettings
    {
        [JsonRequired]
        public string AppLogs { get; set; }

        [JsonRequired]
        public string Permissions { get; set; }
    }
}