using System;
using System.Collections.Generic;
using System.Text;

namespace DC.Web.Ui.Services.JobQueue
{
    public class Job
    {
        public long JobId { get; set; }

        public string FileName { get; set; }

        public long Ukprn { get; set; }

        public DateTime DateTimeSubmittedUtc { get; set; }

        public string StorageReference { get; set; }
    }
}
