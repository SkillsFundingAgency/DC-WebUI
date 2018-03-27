using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Web.Ui.Services.Models
{
    public class AppLog
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public string Exception { get; set; }
        public DateTime? TimeStampUtc { get; set; }
        public string MachineName { get; set; }
        public string ProcessName { get; set; }
        public string CallerName { get; set; }
        public string SourceFile { get; set; }
        public int? LineNumber { get; set; }
        public string JobId { get; set; }
        public string TaskKey { get; set; }
    }
}
