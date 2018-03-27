using System;

namespace DC.Web.Ui.ViewModels
{
    public class IlrFileViewModel
    {
        public string Filename;
        public Guid CorrelationId;
        public DateTime SubmissionDateTime { get; set; }
        public decimal FileSize { get; set; }
    }
}
