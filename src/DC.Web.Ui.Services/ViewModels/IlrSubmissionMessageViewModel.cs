using System;

namespace DC.Web.Ui.Services.ViewModels
{
    public class IlrSubmissionMessageViewModel
    {
        public string Filename { get; set; }

        public string ContainerReference { get; set; }

        public Guid CorrelationId { get; set; }
    }
}